using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotBahn.Common;
using DotBahn.Facilities;
using DotBahn.Facilities.Models;
using DotBahn.Facilities.Models.Enumerations;
using DotBahn.Stations;
using DotBahn.Stations.Models;
using DotBahn.Stations.Models.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;

return await StationBrowserCli.BuildRootCommand().Parse(args).InvokeAsync();

internal static class StationBrowserCli {
    private static readonly Argument<string?> SearchArgument = new("search") {
        Description = "Station name to search for (e.g. würzburg)",
        Arity = ArgumentArity.ZeroOrOne,
    };

    private static readonly Option<string?> ClientIdOption = new("--client") {
        Description = "DB API client id; falls back to the DOTBAHN_CLIENT environment variable",
    };

    private static readonly Option<string?> SecretOption = new("--secret") {
        Description = "DB API secret; falls back to the DOTBAHN_SECRET environment variable",
    };

    private static readonly Option<bool> JsonOption = new("--json", "-j") {
        Description = "Print the matched stations as JSON instead of starting the interactive browser",
    };

    internal static RootCommand BuildRootCommand() {
        var rootCommand = new RootCommand("Interactive browser for Deutsche Bahn stations and their facilities") {
            SearchArgument,
            ClientIdOption,
            SecretOption,
            JsonOption,
        };

        rootCommand.Validators.Add(result => {
            var search = result.GetValue(SearchArgument);
            if (string.IsNullOrWhiteSpace(search)) {
                result.AddError("A search name is required: pass the station name to look up.");
            }
        });

        rootCommand.SetAction(async (parseResult, cancellationToken) => {
            var search = parseResult.GetValue(SearchArgument)!;
            var clientId = parseResult.GetValue(ClientIdOption);
            var secret = parseResult.GetValue(SecretOption);
            var json = parseResult.GetValue(JsonOption);

            return await RunAsync(search, clientId, secret, json, cancellationToken);
        });

        return rootCommand;
    }

    private static async Task<int> RunAsync(string search, string? clientId, string? secret, bool json, CancellationToken cancellationToken) {
        if (!ResolveCredentials(clientId, secret, out var resolvedClientId, out var resolvedSecret)) {
            if (json) {
                WriteJsonError("missing-credentials", "Provide --client/--secret or set DOTBAHN_CLIENT and DOTBAHN_SECRET.");
            } else {
                AnsiConsole.MarkupLine("[red]Usage:[/] provide --client/--secret, or set DOTBAHN_CLIENT / DOTBAHN_SECRET via env or CLI args");
            }
            return 1;
        }

        List<Station> stations;

        try {
            stations = await FindStationsAsync(resolvedClientId, resolvedSecret, search, cancellationToken);
        } catch (HttpRequestException ex) {
            if (json) {
                WriteJsonError("api-error", ex.Message);
            } else {
                AnsiConsole.MarkupLine($"[red]API request failed:[/] [grey]{Markup.Escape(ex.Message)}[/]");
            }
            return 1;
        }

        if (stations.Count == 0) {
            if (json) {
                WriteJsonError("station-not-found", $"No stations found matching \"{search}\"");
            } else {
                AnsiConsole.MarkupLine($"[yellow]No stations found matching '{Markup.Escape(search)}'[/]");
            }
            return 1;
        }

        if (json) {
            Console.WriteLine(JsonSerializer.Serialize(new StationsPayload([.. stations.Select(ToStationDto)]), StationBrowserJsonContext.Default.StationsPayload));
            return 0;
        }

        await BrowseAsync(stations, resolvedClientId, resolvedSecret);
        return 0;
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

    private static async Task<List<Station>> FindStationsAsync(string clientId, string secret, string search, CancellationToken cancellationToken) {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddDotBahnAuthorization(opt => {
            opt.ClientId = clientId;
            opt.ApiKey = secret;
        });
        services.AddDotBahnStations(opt => opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/"));
        services.AddDotBahnFacilities(opt => opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/"));

        var serviceProvider = services.BuildServiceProvider();
        var stationsClient = serviceProvider.GetRequiredService<IStationClient>();

        var stations = new List<Station>();
        var query = new StationQuery().WithNames(search);

        await AnsiConsole.Status().Spinner(Spinner.Known.Dots).StartAsync($"Searching for stations matching '{search}'...", async _ => {
            stations.AddRange(await stationsClient.GetStationsAsync(query, cancellationToken));
        });

        return stations;
    }

    private static StationDto ToStationDto(Station station) => new(
        station.Number,
        station.Name,
        station.PrimaryEva?.Number,
        station.Category.ToString(),
        station.Coordinates?.Latitude,
        station.Coordinates?.Longitude);

    private static void WriteJsonError(string code, string message) {
        var payload = new ErrorPayload(new ErrorDto(code, message));
        Console.WriteLine(JsonSerializer.Serialize(payload, StationBrowserJsonContext.Default.ErrorPayload));
    }

    private static async Task BrowseAsync(List<Station> stations, string clientId, string secret) {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddDotBahnAuthorization(opt => {
            opt.ClientId = clientId;
            opt.ApiKey = secret;
        });
        services.AddDotBahnStations(opt => opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/"));
        services.AddDotBahnFacilities(opt => opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/"));

        var serviceProvider = services.BuildServiceProvider();
        var facilitiesClient = serviceProvider.GetRequiredService<IFacilityClient>();

        AnsiConsole.MarkupLine($"[green]Found {stations.Count} station(s)[/]");
        var facilitiesCache = new Dictionary<int, List<Facility>>();

        var currentIndex = 0;
        while (true) {
            var currentStation = stations[currentIndex];

            if (!facilitiesCache.TryGetValue(currentStation.Number, out var facilities)) {
                await AnsiConsole.Status().Spinner(Spinner.Known.Dots).StartAsync(
                    "Loading facilities...",
                    async _ => {
                        var query = new FacilityQuery().AtStation(currentStation.Number);
                        facilities = (await facilitiesClient.GetFacilitiesAsync(query)).ToList();
                        facilitiesCache[currentStation.Number] = facilities;
                    });
            }

            AnsiConsole.Clear();
            RenderStation(currentStation, facilities ?? [], currentIndex, stations.Count);
            RenderNavigation();

            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.LeftArrow:
                    if (currentIndex > 0) {
                        currentIndex--;
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (currentIndex < stations.Count - 1) {
                        currentIndex++;
                    }

                    break;
                case ConsoleKey.Home:
                    currentIndex = 0;
                    break;
                case ConsoleKey.End:
                    currentIndex = stations.Count - 1;
                    break;
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    return;
            }
        }
    }

    private static void RenderStation(Station station, List<Facility> facilities, int index, int total) {
        var rule = new Rule(Markup.Escape(station.Name)) {
            Justification = Justify.Center,
        };

        AnsiConsole.Write(rule);
        RenderPositionBar(index, total);
        AnsiConsole.WriteLine();

        var leftPanel = BuildStationInfo(station);
        var rightPanel = BuildRightPanel(station.Services, facilities);

        var layout = new Columns(leftPanel, rightPanel) {
            Expand = true
        };

        AnsiConsole.Write(layout);
    }

    private static IRenderable BuildStationInfo(Station station) {
        var rows = new List<IRenderable>();

        var infoGrid = new Grid();
        infoGrid.AddColumn(new GridColumn().Width(18));
        infoGrid.AddColumn();

        infoGrid.AddRow("[grey]Station Number[/]", $"{station.Number}");
        infoGrid.AddRow("[grey]Category[/]", FormatCategory(station.Category));

        if (station.PrimaryEva != null) {
            infoGrid.AddRow("[grey]EVA Number[/]", $"{station.PrimaryEva.Number}");
        }

        if (station.PrimaryRil100 != null) {
            infoGrid.AddRow("[grey]RIL100[/]", Markup.Escape(station.PrimaryRil100.Identifier));
        }

        if (station.Coordinates != null) {
            var c = station.Coordinates.Value;
            infoGrid.AddRow("[grey]Coordinates[/]", $"{c.Latitude:F6}, {c.Longitude:F6}");
        }

        if (station.RegionalArea != null) {
            infoGrid.AddRow("[grey]Regional Area[/]", Markup.Escape($"{station.RegionalArea.Name} ({station.RegionalArea.ShortName})"));
        }

        rows.Add(infoGrid);
        rows.Add(new Text(""));

        if (station.Address != null) {
            var addressPanel = new Panel(
                Markup.Escape($"{station.Address.Street}\n{station.Address.ZipCode} {station.Address.City}")
            ) {
                Header = new PanelHeader("[blue]Address[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("grey"),
                Expand = false
            };
            rows.Add(addressPanel);
            rows.Add(new Text(""));
        }

        if (station.EvaNumbers.Count() > 1) {
            rows.Add(new Markup("[grey]EVA Numbers:[/]"));
            rows.AddRange(station.EvaNumbers.Select(eva => new Text($"  {eva.Number}{(eva.IsMain ? " (main)" : "")}")).Cast<IRenderable>());
            rows.Add(new Text(""));
        }

        if (station.Ril100Identifiers.Count() <= 1) {
            return new Rows(rows);
        }

        rows.Add(new Markup("[grey]RIL100 Identifiers:[/]"));
        foreach (var ril in station.Ril100Identifiers) {
            rows.Add(new Text($"  {Markup.Escape(ril.Identifier)}{(ril.IsMain ? " (main)" : "")}"));
        }

        return new Rows(rows);
    }

    private static IRenderable BuildRightPanel(StationServices services, List<Facility> facilities) {
        var rows = new List<IRenderable>();

        var servicesTable = new Table {
            Border = TableBorder.Rounded,
            BorderStyle = Style.Parse("grey")
        };
        servicesTable.AddColumn(new TableColumn("[bold blue]Service[/]").LeftAligned());
        servicesTable.AddColumn(new TableColumn("[blue]?[/]").Centered());

        AddServiceRow(servicesTable, "Parking", services.HasParking);
        AddServiceRow(servicesTable, "Bicycle Parking", services.HasBicycleParking);
        AddServiceRow(servicesTable, "Public Facilities", services.HasPublicFacilities);
        AddServiceRow(servicesTable, "Locker System", services.HasLockerSystem);
        AddServiceRow(servicesTable, "Step-Free Access", services.HasStepFreeAccess);
        AddServiceRow(servicesTable, "Stepless Access", services.HasSteplessAccess);
        AddServiceRow(servicesTable, "Travel Center", services.HasTravelCenter);
        AddServiceRow(servicesTable, "Travel Necessities", services.HasTravelNecessities);
        AddServiceRow(servicesTable, "Mobility Service", services.HasMobilityService);
        AddServiceRow(servicesTable, "WiFi", services.HasWiFi);

        rows.Add(servicesTable);

        if (facilities.Count <= 0) {
            return new Rows(rows);
        }

        rows.Add(new Text(""));
        rows.Add(BuildFacilitiesPanel(facilities));

        return new Rows(rows);
    }

    private static IRenderable BuildFacilitiesPanel(List<Facility> facilities) {
        var elevators = facilities.Where(f => f.Type == FacilityType.Elevator).ToList();
        var escalators = facilities.Where(f => f.Type == FacilityType.Escalator).ToList();

        var table = new Table {
            Border = TableBorder.Rounded,
            BorderStyle = Style.Parse("grey")
        };
        table.AddColumn(new TableColumn("[bold blue]Facilities[/]").LeftAligned());
        table.AddColumn(new TableColumn("[blue]Status[/]").Centered());

        var elevatorsActive = elevators.Count(f => f.State == FacilityState.Active);
        var escalatorsActive = escalators.Count(f => f.State == FacilityState.Active);

        if (elevators.Count > 0) {
            var color = elevatorsActive == elevators.Count ? "green" :
                        elevatorsActive == 0 ? "red" : "yellow";
            table.AddRow(
                "Elevators",
                $"[{color}]{elevatorsActive}/{elevators.Count}[/]"
            );
        }

        if (escalators.Count > 0) {
            var color = escalatorsActive == escalators.Count ? "green" :
                        escalatorsActive == 0 ? "red" : "yellow";
            table.AddRow(
                "Escalators",
                $"[{color}]{escalatorsActive}/{escalators.Count}[/]"
            );
        }

        var inactiveFacilities = facilities.Where(f => f.State != FacilityState.Active).ToList();
        if (inactiveFacilities.Count <= 0) {
            return table;
        }

        table.AddEmptyRow();
        table.AddRow("[red]Out of service:[/]", "");
        foreach (var facility in inactiveFacilities.Take(5)) {
            var typeIcon = facility.Type == FacilityType.Elevator ? "\u21c5" : "\u21f5";
            var desc = facility.Description ?? $"#{facility.EquipmentNumber}";
            if (desc.Length > 25) {
                desc = desc[..22] + "...";
            }

            table.AddRow($"[grey]{typeIcon} {Markup.Escape(desc)}[/]", "");
        }
        if (inactiveFacilities.Count > 5) {
            table.AddRow($"[grey]... and {inactiveFacilities.Count - 5} more[/]", "");
        }

        return table;
    }

    private static string FormatCategory(StationCategory category) {
        var (color, description) = category switch {
            StationCategory.Category1 => ("green", "Major Hub"),
            StationCategory.Category2 => ("aqua", "Large Station"),
            StationCategory.Category3 => ("blue", "Important Regional"),
            StationCategory.Category4 => ("default", "Regional Station"),
            StationCategory.Category5 => ("grey", "Standard Station"),
            StationCategory.Category6 => ("grey", "Small Station"),
            StationCategory.Category7 => ("grey", "Minor Stop"),
            _ => ("yellow", "Unknown")
        };

        var categoryNum = category switch {
            StationCategory.Category1 => "1",
            StationCategory.Category2 => "2",
            StationCategory.Category3 => "3",
            StationCategory.Category4 => "4",
            StationCategory.Category5 => "5",
            StationCategory.Category6 => "6",
            StationCategory.Category7 => "7",
            _ => "?"
        };

        return $"[bold {color}]{categoryNum}[/] [grey]({description})[/]";
    }

    private static void AddServiceRow(Table table, string name, bool available) {
        var icon = available ? "[green]Yes[/]" : "[red]No[/]";
        table.AddRow(name, icon);
    }

    private static void RenderPositionBar(int index, int total) {
        var segments = new StringBuilder();
        for (var i = 0; i < total; i++) {
            segments.Append(i == index ? "[blue]\u2588[/]" : "[grey]\u2591[/]");
        }

        AnsiConsole.MarkupLine(segments.ToString());
    }

    private static void RenderNavigation() {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule());
        AnsiConsole.MarkupLine("[grey]Navigation: [/]" + "[default]\u2190[/] Previous | [default]\u2192[/] Next | [default]Home[/] First | [default]End[/] Last | [default]Q/Esc[/] Quit");
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
[JsonSerializable(typeof(StationsPayload))]
[JsonSerializable(typeof(ErrorPayload))]
internal sealed partial class StationBrowserJsonContext : JsonSerializerContext;

internal sealed record StationsPayload(IEnumerable<StationDto> Stations);

internal sealed record StationDto(int Number, string Name, long? Eva, string? Category, double? Latitude, double? Longitude);

internal sealed record ErrorPayload(ErrorDto Error);

internal sealed record ErrorDto(string Code, string Message);
