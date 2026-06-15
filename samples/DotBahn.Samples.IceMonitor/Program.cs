using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Timetables;
using DotBahn.Data.Shared.Models;
using DotBahn.Data.Timetables.Enumerations;
using DotBahn.Data.Timetables.Models;
using DotBahn.Modules.Authorization;
using DotBahn.Samples.Shared;

using Spectre.Console;

using static DotBahn.Samples.Shared.ConsoleExtensions;

string? clientId;
string? clientSecret;

if (Credentials.TryFromEnvironment(out var envClient, out var envSecret)) {
    clientId = envClient;
    clientSecret = envSecret;
} else {
    clientId = args.Length > 1 ? args[1] : null;
    clientSecret = args.Length > 2 ? args[2] : null;
}

if (clientId == null || clientSecret == null) {
    AnsiConsole.MarkupLine($"[{Gruvbox.Red}]Usage:[/] provide DOTBAHN_CLIENT / DOTBAHN_SECRET via env or CLI args");
    return 1;
}

if (!int.TryParse(args.Length > 0 ? args[0] : "8098160", out var eva)) {
    eva = 8098160;
}

using var client = new TimetablesClient(
    new ClientOptions {
        BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1"),
    },
    new AuthorizationOptions {
        ClientId = clientId,
        ApiKey = clientSecret,
    });

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

Timetable? cached = null;
var lastFetch = DateOnly.MinValue;

while (!cts.Token.IsCancellationRequested) {
    try {
        await StatusAsync("Loading\u2026", async ctx => {
            var today = DateOnly.FromDateTime(DateTime.Now);

            if (cached == null || lastFetch != today) {
                var start = DateTime.Now;
                var stops = new Dictionary<string, TimetableStop>();
                Timetable? first = null;

                for (var h = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                     h < start.AddHours(24);
                     h = h.AddHours(1)) {
                    ctx.Status($"{h:HH:mm}\u2026");
                    var data = await client.GetTimetableAsync(eva, h);
                    first ??= data;
                    foreach (var s in data.Stops) stops[s.Id] = s;
                }

                cached = new Timetable {
                    Station = first?.Station ?? $"EVA {eva}",
                    Stops = stops.Values,
                    Messages = [],
                };
                lastFetch = today;
            }

            ctx.Status("Changes\u2026");
            cached = await client.GetFullChangesAsync(eva, cached);

            var departures = cached.Stops
                .Where(s => "ICE".Equals(s.Train.Category, StringComparison.OrdinalIgnoreCase))
                .Where(s => s.Departure is { Status.Actual: not EventStatus.Cancelled })
                .Where(s => s.Departure!.Time.Actual >= DateTime.Now)
                .OrderBy(s => s.Departure!.Time.Actual)
                .Take(20)
                .ToList();

            ctx.Status("Render\u2026");
            AnsiConsole.Clear();
            RenderHeader(cached.Station, eva);
            RenderDepartures(departures);
        });

        AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Next refresh in 2 min. Ctrl+C to exit.[/]");
        await Task.Delay(TimeSpan.FromMinutes(2), cts.Token);
    } catch (OperationCanceledException) {
        break;
    } catch (HttpRequestException ex) {
        AnsiConsole.MarkupLine($"[{Gruvbox.Red}]Error:[/] {ex.Message}");
        await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
    }
}

return 0;

static void RenderHeader(string station, int eva) {
    AnsiConsole.Write(TitleRule($"ICE Departures \u2013 {Markup.Escape(station)}"));
    AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]EVA {eva} | {DateTime.Now:HH:mm}[/]\n");
}

static void RenderDepartures(List<TimetableStop> stops) {
    if (stops.Count == 0) {
        AnsiConsole.MarkupLine($"[{Gruvbox.Yellow}]No ICE departures found.[/]");
        return;
    }

    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(BorderColor)
        .AddColumn("[bold]Train[/]")
        .AddColumn("[bold]Departure[/]")
        .AddColumn("[bold]Platf.[/]")
        .AddColumn("[bold]Destination[/]")
        .AddColumn("[bold]Via[/]");

    foreach (var stop in stops) {
        var dep = stop.Departure!;
        table.AddRow(
            FormatTrain(stop.Train),
            FormatTime(dep.Time),
            FormatPlatform(dep.Platform),
            FormatDestination(dep.Path),
            FormatVia(dep.Path));
    }

    AnsiConsole.Write(table);
    AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Showing {stops.Count} ICE departure(s)[/]\n");
}

static string FormatTrain(TrainLabel t) =>
    $"[bold {Gruvbox.Fg}]{Markup.Escape(t.DisplayName)}[/]";

static string FormatTime(ChangedValue<DateTime> t) {
    var p = t.Original.ToString("HH:mm");
    if (!t.HasUpdate) return $"[{Gruvbox.Green}]{p}[/]";
    var a = t.Actual.ToString("HH:mm");
    var d = (int)(t.Actual - t.Original).TotalMinutes;
    return d <= 0
        ? $"[{Gruvbox.Green}]{a}[/]"
        : $"[strikethrough {Gruvbox.Gray}]{p}[/] [bold {Gruvbox.Red}]{a} (+{d})[/]";
}

static string FormatPlatform(ChangedRef<string> p) {
    if (!p.HasUpdate || p.Updated == p.Original)
        return $"[{Gruvbox.Fg}]{Markup.Escape(p.Original)}[/]";
    return $"[strikethrough {Gruvbox.Gray}]{Markup.Escape(p.Original)}[/] [bold {Gruvbox.Red}]{Markup.Escape(p.Actual)}[/]";
}

static string FormatDestination(ChangedRef<IEnumerable<string>> p) {
    var planned = p.Original.LastOrDefault() ?? "-";
    var actual = p.Actual.LastOrDefault() ?? "-";
    if (!p.HasUpdate || planned == actual)
        return $"[bold {Gruvbox.Fg}]{Markup.Escape(planned)}[/]";
    return $"[strikethrough {Gruvbox.Gray}]{Markup.Escape(planned)}[/] [bold {Gruvbox.Red}]{Markup.Escape(actual)}[/]";
}

static string FormatVia(ChangedRef<IEnumerable<string>> p) {
    var stops = p.Actual?.ToList() ?? [];
    if (stops.Count <= 1) return $"[{Gruvbox.Gray}]-[/]";
    var via = string.Join(" \u2013 ", stops.Take(Math.Min(3, stops.Count - 1)).Select(Markup.Escape));
    if (stops.Count > 4) via += " \u2026";
    return p.HasUpdate
        ? $"[{Gruvbox.Orange}]{via}[/]"
        : $"[{Gruvbox.Gray}]{via}[/]";
}


