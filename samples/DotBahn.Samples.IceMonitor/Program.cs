using DotBahn.Clients.Timetables;
using DotBahn.Clients.Timetables.Client;
using DotBahn.Data.Timetables.Enumerations;
using DotBahn.Data.Timetables.Models;
using DotBahn.Data.Timetables.Models.Base;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

if (args.Length < 3) {
    AnsiConsole.MarkupLine($"[{Gruvbox.Red}]Usage:[/] DotBahn.Samples.IceMonitor <EVA> <ClientId> <ClientSecret>");
    AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Example: DotBahn.Samples.IceMonitor 8000105 your-client-id your-api-key[/]");
    return 1;
}

if (!int.TryParse(args[0], out var eva)) {
    AnsiConsole.MarkupLine($"[{Gruvbox.Red}]Error:[/] EVA must be a valid number.");
    return 1;
}

var clientId = args[1];
var clientSecret = args[2];

// Setup DI
var services = new ServiceCollection();
services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
services.AddDotBahnAuthorization(opt => {
    opt.ClientId = clientId;
    opt.ApiKey = clientSecret;
});
services.AddDotBahnCache(opt => opt.DefaultExpiration = TimeSpan.FromSeconds(30));
services.AddDotBahnTimetables(opt => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1");
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<TimetablesClient>();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => {
    e.Cancel = true;
    cts.Cancel();
};

Timetable? cachedTimetable = null;
var lastFetchDate = DateOnly.MinValue;

while (!cts.Token.IsCancellationRequested) {
    try {
        await AnsiConsole.Status()
                         .Spinner(Spinner.Known.Dots)
                         .StartAsync("Loading timetable data...", async ctx => {
                             var today = DateOnly.FromDateTime(DateTime.Now);

                             if (cachedTimetable == null || lastFetchDate != today) {
                                 ctx.Status("Fetching scheduled timetable...");
                                 cachedTimetable = await client.GetTimetableAsync(eva, DateTime.Now);
                                 lastFetchDate = today;
                             }

                             ctx.Status("Fetching changes...");
                             cachedTimetable = await client.GetFullChangesAsync(eva, cachedTimetable);

                             var now = DateTime.Now;
                             var iceStops = cachedTimetable.Stops
                                                           .Where(s => s.Train.Category?.Equals("ICE", StringComparison.OrdinalIgnoreCase) == true)
                                                           .Where(s => s.Departure != null)
                                                           .Where(s => s.Departure!.Status.Actual != EventStatus.Cancelled)
                                                           .Where(s => s.Departure!.Time.Actual >= now)
                                                           .OrderBy(s => s.Departure!.Time.Actual)
                                                           .Take(20)
                                                           .ToList();

                             ctx.Status("Rendering...");

                             AnsiConsole.Clear();
                             RenderHeader(cachedTimetable.Station, eva);
                             RenderDepartures(iceStops);
                         });

        AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Next refresh in 2 minutes. Press Ctrl+C to exit.[/]");
        await Task.Delay(TimeSpan.FromMinutes(2), cts.Token);
    } catch (HttpRequestException ex) {
        AnsiConsole.MarkupLine($"[{Gruvbox.Red}]API Error:[/] {ex.Message}");
        AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Retrying in 30 seconds...[/]");
        try {
            await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
        } catch (OperationCanceledException) {
            break;
        }
    } catch (OperationCanceledException) {
        break;
    }
}

AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Goodbye![/]");
return 0;

static void RenderHeader(string station, int eva) {
    var rule = new Rule($"[bold {Gruvbox.Blue}]ICE Departures - {Markup.Escape(station)}[/]") {
        Justification = Justify.Center,
        Style = Style.Parse(Gruvbox.Blue)
    };
    AnsiConsole.Write(rule);
    AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]EVA: {eva} | Updated: {DateTime.Now:HH:mm:ss}[/]");
    AnsiConsole.WriteLine();
}

static void RenderDepartures(List<TimetableStop> stops) {
    if (stops.Count == 0) {
        AnsiConsole.MarkupLine($"[{Gruvbox.Yellow}]No ICE departures found.[/]");
        return;
    }

    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(new Color(0x92, 0x83, 0x74))
        .AddColumn(new TableColumn($"[bold {Gruvbox.Fg}]Train[/]").LeftAligned())
        .AddColumn(new TableColumn($"[bold {Gruvbox.Fg}]Departure[/]").LeftAligned())
        .AddColumn(new TableColumn($"[bold {Gruvbox.Fg}]Platform[/]").Centered())
        .AddColumn(new TableColumn($"[bold {Gruvbox.Fg}]Destination[/]").LeftAligned())
        .AddColumn(new TableColumn($"[bold {Gruvbox.Fg}]Via[/]").LeftAligned());

    foreach (var stop in stops) {
        var departure = stop.Departure!;

        var trainDisplay = FormatTrain(stop.Train);
        var timeDisplay = FormatTime(departure.Time);
        var platformDisplay = FormatPlatform(departure.Platform);
        var destinationDisplay = FormatDestination(departure.Path);
        var viaDisplay = FormatVia(departure.Path);

        table.AddRow(trainDisplay, timeDisplay, platformDisplay, destinationDisplay, viaDisplay);
    }

    AnsiConsole.Write(table);
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Showing {stops.Count} ICE departure(s)[/]");
}

static string FormatTrain(TrainLabel train) {
    return $"[bold {Gruvbox.Fg}]{Markup.Escape(train.DisplayName)}[/]";
}

static string FormatTime(ChangedValue<DateTime> time) {
    var planned = time.Original.ToString("HH:mm");

    if (!time.HasUpdate) {
        return $"[{Gruvbox.Green}]{planned}[/]";
    }

    var actual = time.Actual.ToString("HH:mm");
    var delay = (int)(time.Actual - time.Original).TotalMinutes;

    if (delay <= 0) {
        return $"[{Gruvbox.Green}]{actual}[/]";
    }

    var delayText = $"+{delay}";
    return $"[strikethrough {Gruvbox.Gray}]{planned}[/] [bold {Gruvbox.Red}]{actual}[/] [{Gruvbox.Red}]({delayText})[/]";
}

static string FormatPlatform(ChangedRef<string> platform) {
    var planned = platform.Original;

    if (!platform.HasUpdate || platform.Updated == platform.Original) {
        return $"[{Gruvbox.Fg}]{Markup.Escape(planned)}[/]";
    }

    var actual = platform.Actual;
    return $"[strikethrough {Gruvbox.Gray}]{Markup.Escape(planned)}[/] [bold {Gruvbox.Red}]{Markup.Escape(actual)}[/]";
}

static string FormatDestination(ChangedRef<IEnumerable<string>> path) {
    var plannedPath = path.Original.ToList();
    var actualPath = path.Actual.ToList();

    var plannedDest = plannedPath.Count > 0 ? plannedPath[^1] : "-";
    var actualDest = actualPath.Count > 0 ? actualPath[^1] : "-";

    if (!path.HasUpdate || plannedDest == actualDest) {
        return $"[bold {Gruvbox.Fg}]{Markup.Escape(plannedDest)}[/]";
    }

    return $"[strikethrough {Gruvbox.Gray}]{Markup.Escape(plannedDest)}[/] [bold {Gruvbox.Red}]{Markup.Escape(actualDest)}[/]";
}

static string FormatVia(ChangedRef<IEnumerable<string>> path) {
    var actualPath = path.Actual?.ToList() ?? [];
    if (actualPath.Count <= 1) {
        return $"[{Gruvbox.Gray}]-[/]";
    }
    
    var viaStops = actualPath.Take(Math.Min(3, actualPath.Count - 1)).ToList();
    var viaText = string.Join(" - ", viaStops.Select(Markup.Escape));

    if (actualPath.Count > 4) {
        viaText += " ...";
    }
    
    if (path.HasUpdate) {
        return $"[{Gruvbox.Orange}]{viaText}[/]";
    }

    return $"[{Gruvbox.Gray}]{viaText}[/]";
}

internal static class Gruvbox {
    public const string Fg = "#ebdbb2";
    public const string Red = "#fb4934";
    public const string Green = "#b8bb26";
    public const string Yellow = "#fabd2f";
    public const string Blue = "#83a598";
    public const string Aqua = "#8ec07c";
    public const string Orange = "#fe8019";
    public const string Gray = "#928374";
}
