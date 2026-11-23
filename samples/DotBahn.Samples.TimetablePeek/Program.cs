using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotBahn.Boards;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Models;
using DotBahn.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Spectre.Console;

return await TimetablePeekCli.BuildRootCommand().Parse(args).InvokeAsync();

internal static class TimetablePeekCli {
    private static readonly Uri BaseEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/open-boards/v1/");

    private static readonly Argument<string?> EvaArgument = new("eva") {
        Description = "EVA station number (e.g. 8098160)",
        Arity = ArgumentArity.ZeroOrOne,
    };

    private static readonly Option<bool> ArrivalsOption = new("--arrivals", "-a") {
        Description = "Show arrivals instead of departures",
    };

    private static readonly Option<string?> CategoryOption = new("--category", "-c") {
        Description = "Train category to filter for (e.g. ICE, RB); defaults to all categories",
    };

    private static readonly Option<int> LimitOption = new("--limit", "-n") {
        Description = "Maximum number of entries to show (1-100)",
        DefaultValueFactory = _ => 20,
    };

    private static readonly Option<int> WindowOption = new("--window", "-w") {
        Description = "Time window for the board, in hours (1-8)",
        DefaultValueFactory = _ => 8,
    };

    private static readonly Option<string?> ClientIdOption = new("--client") {
        Description = "DB API client id; falls back to the DOTBAHN_CLIENT environment variable",
    };

    private static readonly Option<string?> SecretOption = new("--secret") {
        Description = "DB API secret; falls back to the DOTBAHN_SECRET environment variable",
    };

    private static readonly Option<bool> JsonOption = new("--json", "-j") {
        Description = "Print entries as JSON instead of a table",
    };

    internal static RootCommand BuildRootCommand() {
        var rootCommand = new RootCommand("Deutsche Bahn arrival/departure board (OpenBoards API)") {
            EvaArgument,
            ArrivalsOption,
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
            } else if (window > 8) {
                result.AddError("--window must be at most 8 (API limit).");
            }
        });

        rootCommand.SetAction(async (parseResult, cancellationToken) => {
            var eva = parseResult.GetValue(EvaArgument);
            var arrivals = parseResult.GetValue(ArrivalsOption);
            var category = parseResult.GetValue(CategoryOption);
            var limit = parseResult.GetValue(LimitOption);
            var window = parseResult.GetValue(WindowOption);
            var clientId = parseResult.GetValue(ClientIdOption);
            var secret = parseResult.GetValue(SecretOption);
            var json = parseResult.GetValue(JsonOption);

            return await RunAsync(eva, arrivals, category, limit, window, clientId, secret, json, cancellationToken);
        });

        return rootCommand;
    }

    private static async Task<int> RunAsync(string? eva, bool arrivals, string? category, int limit, int window, string? clientId, string? secret, bool json, CancellationToken cancellationToken) {
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

        using var client = new BoardsClient(
            new ClientOptions {
                BaseEndpoint = BaseEndpoint,
            },
            new AuthorizationOptions {
                ClientId = resolvedClientId,
                ApiKey = resolvedSecret,
            });

        try {
            var query = new BoardQuery()
                .ForEvaNumbers(evaNumber.ToString(CultureInfo.InvariantCulture))
                .StartingAt(DateTime.Now)
                .EndingAt(DateTime.Now.AddHours(window))
                .WithPlatformAccessibility();

            var entries = new List<BoardEntryDto>();
            string station;

            if (arrivals) {
                var board = await client.GetArrivalsAsync(query, cancellationToken);
                station = BoardStation(board.ConsideredStopPlaces, evaNumber);
                entries.AddRange(board.Entries
                    .Where(e => Matches(e.Transport.Category, category))
                    .Select(ToArrivalDto));
            } else {
                var board = await client.GetDeparturesAsync(query, cancellationToken);
                station = BoardStation(board.ConsideredStopPlaces, evaNumber);
                entries.AddRange(board.Entries
                    .Where(e => Matches(e.Transport.Category, category))
                    .Select(ToDepartureDto));
            }

            entries = entries
                .Where(e => e.Actual >= DateTime.Now)
                .OrderBy(e => e.Actual)
                .Take(limit)
                .ToList();

            if (json) {
                var payload = new BoardPayload(evaNumber, station, arrivals ? "arrivals" : "departures", entries);
                Console.WriteLine(JsonSerializer.Serialize(payload, TimetablePeekJsonContext.Default.BoardPayload));
                return entries.Count > 0 ? 0 : 1;
            }

            AnsiConsole.Clear();
            RenderHeader(station, evaNumber, arrivals);
            if (entries.Count == 0) {
                AnsiConsole.MarkupLine($"[yellow]No {(arrivals ? "arrivals" : "departures")} found.[/]");
                return 1;
            }

            RenderBoard(entries, arrivals);
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

    private static bool Matches(string category, string? filter) =>
        filter is null || filter.Equals(category, StringComparison.OrdinalIgnoreCase);

    private static string BoardStation(IReadOnlyList<StopPlace> stopPlaces, int eva) =>
        stopPlaces.FirstOrDefault()?.Name ?? $"EVA {eva}";

    private static BoardEntryDto ToArrivalDto(ArrivalEntry entry) => CreateDto(
        entry, entry.Journey.Origin.StopPlace.Name);

    private static BoardEntryDto ToDepartureDto(DepartureEntry entry) => CreateDto(
        entry, entry.Journey.Destination.StopPlace.Name);

    private static BoardEntryDto CreateDto(BoardEntry entry, string stationName) {
        var time = entry.Time;
        return new BoardEntryDto(
            Train: $"{entry.Transport.Category} {entry.Transport.Number}",
            Category: entry.Transport.Category,
            Planned: time.Original,
            Actual: time.Actual,
            DelayMinutes: (int)(time.Actual - time.Original).TotalMinutes,
            Platform: entry.Platform.Actual.Name,
            Station: stationName,
            Via: entry.Transport.Via
                .Take(Math.Max(0, entry.Transport.Via.Count - 1))
                .Select(v => v.StopPlace.Name)
                .ToList());
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

    private static void WriteJsonError(string code, string message) {
        var payload = new ErrorPayload(new ErrorDto(code, message));
        Console.WriteLine(JsonSerializer.Serialize(payload, TimetablePeekJsonContext.Default.ErrorPayload));
    }

    private static void RenderHeader(string station, int eva, bool arrivals) {
        var title = arrivals ? "Arrivals" : "Departures";
        AnsiConsole.Write(new Rule($"{title} \u2013 {Markup.Escape(station)}") {
            Justification = Justify.Center,
        });
        AnsiConsole.MarkupLine($"[grey]EVA {eva} | {DateTime.Now:HH:mm}[/]\n");
    }

    private static void RenderBoard(List<BoardEntryDto> entries, bool arrivals) {
        var stationHeader = arrivals ? "[bold]Origin[/]" : "[bold]Destination[/]";
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn("[bold]Train[/]")
            .AddColumn(arrivals ? "[bold]Arrival[/]" : "[bold]Departure[/]")
            .AddColumn("[bold]Platf.[/]")
            .AddColumn(stationHeader)
            .AddColumn("[bold]Via[/]");

        foreach (var entry in entries) {
            table.AddRow(
                FormatTrain(entry.Train),
                FormatTime(entry.Planned, entry.Actual, entry.DelayMinutes),
                Markup.Escape(entry.Platform ?? "-"),
                $"[bold]{Markup.Escape(entry.Station)}[/]",
                FormatVia(entry.Via));
        }

        AnsiConsole.Write(table);
        var label = arrivals ? "arrival(s)" : "departure(s)";
        AnsiConsole.MarkupLine($"[grey]Showing {entries.Count} {label}[/]\n");
    }

    private static string FormatTrain(string trainName) => $"[bold]{Markup.Escape(trainName)}[/]";

    private static string FormatTime(DateTime planned, DateTime actual, int delay) {
        var p = planned.ToString("HH:mm", CultureInfo.InvariantCulture);
        if (delay <= 0) {
            return $"[green]{p}[/]";
        }

        var a = actual.ToString("HH:mm", CultureInfo.InvariantCulture);
        return $"[strikethrough grey]{p}[/] [bold red]{a} (+{delay})[/]";
    }

    private static string FormatVia(IEnumerable<string> stops) {
        var list = stops.ToList();
        if (list.Count == 0) {
            return "[grey]-[/]";
        }

        var via = string.Join(" \u2013 ", list.Take(3).Select(Markup.Escape));
        if (list.Count > 3) {
            via += " \u2026";
        }

        return $"[grey]{via}[/]";
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
[JsonSerializable(typeof(BoardPayload))]
[JsonSerializable(typeof(ErrorPayload))]
internal sealed partial class TimetablePeekJsonContext : JsonSerializerContext;

internal sealed record BoardPayload(int Eva, string Station, string Board, IEnumerable<BoardEntryDto> Entries);

internal sealed record BoardEntryDto(string Train, string Category, DateTime Planned, DateTime Actual, int DelayMinutes, string? Platform, string Station, IEnumerable<string> Via);

internal sealed record ErrorPayload(ErrorDto Error);

internal sealed record ErrorDto(string Code, string Message);
