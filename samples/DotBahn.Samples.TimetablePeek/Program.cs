using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Models;
using DotBahn.Telemetry;
using DotBahn.Timetables;
using DotBahn.Timetables.Models;
using DotBahn.Timetables.Models.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Spectre.Console;

return await TimetablePeekCli.BuildRootCommand().Parse(args).InvokeAsync();

internal static class TimetablePeekCli {
    private static readonly Uri BaseEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/");

    private static readonly Argument<string?> EvaArgument = new("eva") {
        Description = "EVA station number (e.g. 8098160)",
        Arity = ArgumentArity.ZeroOrOne,
    };

    private static readonly Option<string?> CategoryOption = new("--category", "-c") {
        Description = "Train category to filter for (e.g. ICE, RB); defaults to all categories",
    };

    private static readonly Option<int> LimitOption = new("--limit", "-n") {
        Description = "Maximum number of departures to show (1-100)",
        DefaultValueFactory = _ => 20,
    };

    private static readonly Option<int> WindowOption = new("--window", "-w") {
        Description = "Look-ahead window for the timetable, in hours (1-24)",
        DefaultValueFactory = _ => 24,
    };

    private static readonly Option<string?> ClientIdOption = new("--client") {
        Description = "DB API client id; falls back to the DOTBAHN_CLIENT environment variable",
    };

    private static readonly Option<string?> SecretOption = new("--secret") {
        Description = "DB API secret; falls back to the DOTBAHN_SECRET environment variable",
    };

    private static readonly Option<bool> JsonOption = new("--json", "-j") {
        Description = "Print departures as JSON instead of a table",
    };

    internal static RootCommand BuildRootCommand() {
        var rootCommand = new RootCommand("Deutsche Bahn departure monitor showing the next departures at a station") {
            EvaArgument,
            CategoryOption,
            LimitOption,
            WindowOption,
            ClientIdOption,
            SecretOption,
            JsonOption,
        };

        rootCommand.Validators.Add(result => {
            var eva = result.GetValue(EvaArgument);
            if (string.IsNullOrWhiteSpace(eva)) {
                result.AddError("An EVA station number must be supplied: pass it as first argument (e.g. 8098160).");
            } else if (!int.TryParse(eva, out _)) {
                result.AddError("eva must be a numeric EVA station number (e.g. 8098160).");
            }
        });

        CategoryOption.Validators.Add(result => {
            var category = result.GetValue(CategoryOption);
            if (category is { Length: 0 }) {
                result.AddError("--category must not be empty.");
            }
        });

        LimitOption.Validators.Add(result => {
            var limit = result.GetValue(LimitOption);
            if (limit < 1) {
                result.AddError("--limit must be at least 1.");
            } else if (limit > 100) {
                result.AddError("--limit must be at most 100.");
            }
        });

        WindowOption.Validators.Add(result => {
            var window = result.GetValue(WindowOption);
            if (window < 1) {
                result.AddError("--window must be at least 1.");
            } else if (window > 24) {
                result.AddError("--window must be at most 24.");
            }
        });

        rootCommand.SetAction(async (parseResult, cancellationToken) => {
            var eva = parseResult.GetValue(EvaArgument);
            var category = parseResult.GetValue(CategoryOption);
            var limit = parseResult.GetValue(LimitOption);
            var window = parseResult.GetValue(WindowOption);
            var clientId = parseResult.GetValue(ClientIdOption);
            var secret = parseResult.GetValue(SecretOption);
            var json = parseResult.GetValue(JsonOption);

            return await RunAsync(eva, category, limit, window, clientId, secret, json, cancellationToken);
        });

        return rootCommand;
    }

    private static async Task<int> RunAsync(string? eva, string? category, int limit, int window, string? clientId, string? secret, bool json, CancellationToken cancellationToken) {
        if (!int.TryParse(eva, out var evaNumber)) {
            throw new UnreachableException("eva was validated but could not be parsed.");
        }

        if (!ResolveCredentials(clientId, secret, out var resolvedClientId, out var resolvedSecret)) {
            if (json) {
                WriteJsonError("missing-credentials", "Provide --client/--secret or set DOTBAHN_CLIENT and DOTBAHN_SECRET.");
            } else {
                AnsiConsole.MarkupLine("[red]Usage:[/] provide --client/--secret, or set DOTBAHN_CLIENT / DOTBAHN_SECRET via env or CLI args");
            }
            return 1;
        }

        var services = new ServiceCollection();
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddOtlpExporter())
            .WithMetrics(metrics => metrics.AddOtlpExporter());
        services.AddDotBahnTelemetry();
        await using var telemetry = services.BuildServiceProvider();
        _ = telemetry.GetRequiredService<TracerProvider>();
        _ = telemetry.GetRequiredService<MeterProvider>();

        using var client = new TimetableClient(
            new ClientOptions {
                BaseEndpoint = BaseEndpoint,
            },
            new AuthorizationOptions {
                ClientId = resolvedClientId,
                ApiKey = resolvedSecret,
            });

        try {
            var (timetable, departures) = await FetchAsync(client, evaNumber, category, limit, window, cancellationToken);

            if (json) {
                var payload = new DeparturePayload(evaNumber, timetable.Station, [.. departures.Select(ToDepartureDto)]);
                Console.WriteLine(JsonSerializer.Serialize(payload, TimetablePeekJsonContext.Default.DeparturePayload));
                return departures.Count > 0 ? 0 : 1;
            }

            AnsiConsole.Clear();
            RenderHeader(timetable.Station, evaNumber);
            if (departures.Count == 0) {
                AnsiConsole.MarkupLine("[yellow]No departures found.[/]");
                return 1;
            }

            RenderDepartures(departures);
            return 0;
        } catch (OperationCanceledException) {
            return 130;
        } catch (HttpRequestException ex) {
            if (json) {
                WriteJsonError("api-error", ex.Message);
            } else {
                AnsiConsole.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
            }
            return 1;
        }
    }

    private static bool ResolveCredentials(string? clientId, string? secret, out string resolvedClientId, out string resolvedSecret) {
        _ = TryCredentialsFromEnvironment(out var envClient, out var envSecret);
        resolvedClientId = clientId ?? envClient;
        resolvedSecret = secret ?? envSecret;
        return !string.IsNullOrEmpty(resolvedClientId) && !string.IsNullOrEmpty(resolvedSecret);
    }

    private static bool TryCredentialsFromEnvironment(out string clientId, out string clientSecret) {
        clientId = Environment.GetEnvironmentVariable("DOTBAHN_CLIENT") ?? string.Empty;
        clientSecret = Environment.GetEnvironmentVariable("DOTBAHN_SECRET") ?? string.Empty;
        return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret);
    }

    private static async Task<Timetable> LoadTimetableAsync(TimetableClient client, int eva, int window, CancellationToken cancellationToken) {
        var start = DateTime.Now;
        var stops = new Dictionary<string, TimetableStop>();
        Timetable? first = null;

        for (var h = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0); h < start.AddHours(window); h = h.AddHours(1)) {
            var data = await client.GetTimetableAsync(eva, h, cancellationToken);

            first ??= data;
            foreach (var s in data.Stops) {
                stops[s.Id] = s;
            }
        }

        return new Timetable {
            Station = first?.Station ?? $"EVA {eva}",
            Stops = stops.Values,
            Messages = [],
        };
    }

    private static async Task<(Timetable Timetable, List<TimetableStop> Departures)> FetchAsync(TimetableClient client, int eva, string? category, int limit, int window, CancellationToken cancellationToken) {
        var timetable = await LoadTimetableAsync(client, eva, window, cancellationToken);
        timetable = await client.GetFullChangesAsync(eva, timetable, cancellationToken);
        return (timetable, SelectDepartures(timetable, category, limit));
    }

    private static List<TimetableStop> SelectDepartures(Timetable timetable, string? category, int limit) => timetable.Stops
        .Where(s => category is null || category.Equals(s.Train.Category, StringComparison.OrdinalIgnoreCase))
        .Where(s => s.Departure is { Status.Actual: not EventStatus.Cancelled })
        .Where(s => s.Departure!.Time.Actual >= DateTime.Now)
        .OrderBy(s => s.Departure!.Time.Actual)
        .Take(limit)
        .ToList();

    private static DepartureDto ToDepartureDto(TimetableStop stop) {
        var dep = stop.Departure!;
        return new DepartureDto(
            stop.Train.DisplayName,
            stop.Train.Category,
            dep.Time.Original,
            dep.Time.Actual,
            (int)(dep.Time.Actual - dep.Time.Original).TotalMinutes,
            dep.Platform.HasUpdate ? dep.Platform.Actual : dep.Platform.Original,
            dep.Path.Actual.LastOrDefault(),
            [.. dep.Path.Actual.SkipLast(1)]);
    }

    private static void WriteJsonError(string code, string message) {
        var payload = new ErrorPayload(new ErrorDto(code, message));
        Console.WriteLine(JsonSerializer.Serialize(payload, TimetablePeekJsonContext.Default.ErrorPayload));
    }

    private static void RenderHeader(string station, int eva) {
        AnsiConsole.Write(new Rule($"Departures \u2013 {Markup.Escape(station)}") {
            Justification = Justify.Center,
        });
        AnsiConsole.MarkupLine($"[grey]EVA {eva} | {DateTime.Now:HH:mm}[/]\n");
    }

    private static void RenderDepartures(List<TimetableStop> stops) {
        if (stops.Count == 0) {
            AnsiConsole.WriteLine("No departures found.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn("[bold]Train[/]")
            .AddColumn("[bold]Departure[/]")
            .AddColumn("[bold]Platf.[/]")
            .AddColumn("[bold]Destination[/]")
            .AddColumn("[bold]Via[/]");

        foreach (var stop in stops) {
            var dep = stop.Departure!;
            table.AddRow(FormatTrain(stop.Train.DisplayName), FormatTime(dep.Time), FormatPlatform(dep.Platform), FormatDestination(dep.Path), FormatVia(dep.Path));
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[grey]Showing {stops.Count} departure(s)[/]\n");
    }

    private static string FormatTrain(string trainName) => $"[bold]{Markup.Escape(trainName)}[/]";

    private static string FormatTime(ChangedValue<DateTime> t) {
        var p = t.Original.ToString("HH:mm");
        if (!t.HasUpdate) {
            return $"[green]{p}[/]";
        }

        var a = t.Actual.ToString("HH:mm");
        var d = (int)(t.Actual - t.Original).TotalMinutes;
        return d <= 0 ? $"[green]{a}[/]" : $"[strikethrough grey]{p}[/] [bold red]{a} (+{d})[/]";
    }

    private static string FormatPlatform(ChangedRef<string> p) {
        if (!p.HasUpdate || p.Updated == p.Original) {
            return Markup.Escape(p.Original);
        }

        return $"[strikethrough grey]{Markup.Escape(p.Original)}[/] [bold red]{Markup.Escape(p.Actual)}[/]";
    }

    private static string FormatDestination(ChangedRef<IEnumerable<string>> p) {
        var planned = Markup.Escape(p.Original.LastOrDefault() ?? "-");
        var actual = Markup.Escape(p.Actual.LastOrDefault() ?? "-");
        if (!p.HasUpdate || planned == actual) {
            return $"[bold]{planned}[/]";
        }

        return $"[strikethrough grey]{planned}[/] [bold red]{actual}[/]";
    }

    private static string FormatVia(ChangedRef<IEnumerable<string>> p) {
        var stops = p.Actual.ToList();
        if (stops.Count <= 1) {
            return "[grey]-[/]";
        }

        var via = string.Join(" \u2013 ", stops.Take(Math.Min(3, stops.Count - 1)).Select(Markup.Escape));
        if (stops.Count > 4) {
            via += " \u2026";
        }

        return p.HasUpdate ? $"[orange1]{via}[/]" : $"[grey]{via}[/]";
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
[JsonSerializable(typeof(DeparturePayload))]
[JsonSerializable(typeof(ErrorPayload))]
internal sealed partial class TimetablePeekJsonContext : JsonSerializerContext;

internal sealed record DeparturePayload(int Eva, string Station, IEnumerable<DepartureDto> Departures);

internal sealed record DepartureDto(string Train, string Category, DateTime Planned, DateTime Actual, int DelayMinutes, string? Platform, string? Destination, IEnumerable<string> Via);

internal sealed record ErrorPayload(ErrorDto Error);

internal sealed record ErrorDto(string Code, string Message);
