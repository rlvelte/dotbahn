using System.Text;
using DotBahn.Clients.Facilities;
using DotBahn.Clients.Facilities.Client;
using DotBahn.Clients.Facilities.Query;
using DotBahn.Clients.Stations;
using DotBahn.Clients.Stations.Client;
using DotBahn.Clients.Stations.Query;
using DotBahn.Data.Facilities.Enumerations;
using DotBahn.Data.Facilities.Models;
using DotBahn.Data.Stations.Enumerations;
using DotBahn.Data.Stations.Models;
using DotBahn.Modules.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;

if (args.Length < 3) {
    AnsiConsole.MarkupLine($"[{Gruvbox.Red}]Usage:[/] DotBahn.Samples.StationBrowser <SearchName> <ClientId> <ClientSecret>");
    AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Example: DotBahn.Samples.StationBrowser Berlin your-client-id your-api-key[/]");
    return 1;
}

var searchName = args[0];
var clientId = args[1];
var clientSecret = args[2];

var services = new ServiceCollection();
services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
services.AddDotBahnAuthorization(opt => {
    opt.ClientId = clientId;
    opt.ApiKey = clientSecret;
});
services.AddDotBahnStations(opt => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/");
});
services.AddDotBahnFacilities(opt => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/");
});

var serviceProvider = services.BuildServiceProvider();
var stationsClient = serviceProvider.GetRequiredService<StationsClient>();
var facilitiesClient = serviceProvider.GetRequiredService<FacilitiesClient>();

List<Station> stations = [];
Dictionary<int, List<Facility>> facilitiesCache = [];

await AnsiConsole.Status()
                 .Spinner(Spinner.Known.Dots)
                 .StartAsync($"Searching for stations matching '{searchName}'...", async _ => {
                     var query = new StationsQuery().WithNames(searchName);
                     var result = await stationsClient.GetStationsAsync(query);
                     stations.AddRange(result);
                 });

if (stations.Count == 0) {
    AnsiConsole.MarkupLine($"[{Gruvbox.Yellow}]No stations found matching '{Markup.Escape(searchName)}'[/]");
    return 1;
}

AnsiConsole.MarkupLine($"[{Gruvbox.Green}]Found {stations.Count} station(s)[/]");

var currentIndex = 0;
while (true) {
    var currentStation = stations[currentIndex];

    if (!facilitiesCache.TryGetValue(currentStation.Number, out var facilities)) {
        await AnsiConsole.Status()
                         .Spinner(Spinner.Known.Dots)
                         .StartAsync("Loading facilities...", async _ => {
                             var query = new FacilitiesQuery().AtStation(currentStation.Number);
                             facilities = (await facilitiesClient.GetFacilitiesAsync(query)).ToList();
                             facilitiesCache[currentStation.Number] = facilities;
                         });
    }

    AnsiConsole.Clear();
    RenderStation(currentStation, facilities ?? [], currentIndex, stations.Count);
    RenderNavigation();

    var key = Console.ReadKey(true);

    switch (key.Key) {
        case ConsoleKey.LeftArrow:
            if (currentIndex > 0) currentIndex--;
            break;
        case ConsoleKey.RightArrow:
            if (currentIndex < stations.Count - 1) currentIndex++;
            break;
        case ConsoleKey.Home:
            currentIndex = 0;
            break;
        case ConsoleKey.End:
            currentIndex = stations.Count - 1;
            break;
        case ConsoleKey.Escape:
        case ConsoleKey.Q:
            AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Goodbye![/]");
            return 0;
    }
}

static void RenderStation(Station station, List<Facility> facilities, int index, int total) {
    var rule = new Rule($"[bold {Gruvbox.Blue}]{Markup.Escape(station.Name)}[/]") {
        Justification = Justify.Center,
        Style = Style.Parse(Gruvbox.Blue)
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

static IRenderable BuildStationInfo(Station station) {
    var rows = new List<IRenderable>();

    var infoGrid = new Grid();
    infoGrid.AddColumn(new GridColumn().Width(18));
    infoGrid.AddColumn();

    infoGrid.AddRow($"[{Gruvbox.Gray}]Station Number[/]", $"[{Gruvbox.Fg}]{station.Number}[/]");
    infoGrid.AddRow($"[{Gruvbox.Gray}]Category[/]", FormatCategory(station.Category));

    if (station.PrimaryEva != null) {
        infoGrid.AddRow($"[{Gruvbox.Gray}]EVA Number[/]", $"[{Gruvbox.Fg}]{station.PrimaryEva.Number}[/]");
    }

    if (station.PrimaryRil100 != null) {
        infoGrid.AddRow($"[{Gruvbox.Gray}]RIL100[/]", $"[{Gruvbox.Fg}]{Markup.Escape(station.PrimaryRil100.Identifier)}[/]");
    }

    if (station.Coordinates != null) {
        infoGrid.AddRow($"[{Gruvbox.Gray}]Coordinates[/]",
            $"[{Gruvbox.Fg}]{station.Coordinates.Latitude:F6}, {station.Coordinates.Longitude:F6}[/]");
    }

    if (station.RegionalArea != null) {
        infoGrid.AddRow($"[{Gruvbox.Gray}]Regional Area[/]",
            $"[{Gruvbox.Fg}]{Markup.Escape(station.RegionalArea.Name)} ({station.RegionalArea.ShortName})[/]");
    }

    rows.Add(infoGrid);
    rows.Add(new Text(""));

    if (station.Address != null) {
        var addressPanel = new Panel(
            $"[{Gruvbox.Fg}]{Markup.Escape(station.Address.Street)}\n" +
            $"{Markup.Escape(station.Address.ZipCode)} {Markup.Escape(station.Address.City)}[/]"
        ) {
            Header = new PanelHeader($"[{Gruvbox.Blue}]Address[/]"),
            Border = BoxBorder.Rounded,
            BorderStyle = Style.Parse(Gruvbox.Gray),
            Expand = false
        };
        rows.Add(addressPanel);
        rows.Add(new Text(""));
    }

    if (station.EvaNumbers.Count() > 1) {
        rows.Add(new Markup($"[{Gruvbox.Gray}]EVA Numbers:[/]"));
        foreach (var eva in station.EvaNumbers) {
            rows.Add(new Markup($"  [{Gruvbox.Fg}]{eva.Number}{(eva.IsMain ? " (main)" : "")}[/]"));
        }
        rows.Add(new Text(""));
    }

    if (station.Ril100Identifiers.Count() > 1) {
        rows.Add(new Markup($"[{Gruvbox.Gray}]RIL100 Identifiers:[/]"));
        foreach (var ril in station.Ril100Identifiers) {
            rows.Add(new Markup($"  [{Gruvbox.Fg}]{Markup.Escape(ril.Identifier)}{(ril.IsMain ? " (main)" : "")}[/]"));
        }
    }

    return new Rows(rows);
}

static IRenderable BuildRightPanel(StationServices services, List<Facility> facilities) {
    var rows = new List<IRenderable>();

    var servicesTable = new Table {
        Border = TableBorder.Rounded,
        BorderStyle = Style.Parse(Gruvbox.Gray)
    };
    servicesTable.AddColumn(new TableColumn($"[bold {Gruvbox.Blue}]Service[/]").LeftAligned());
    servicesTable.AddColumn(new TableColumn($"[{Gruvbox.Blue}]?[/]").Centered());

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

    if (facilities.Count > 0) {
        rows.Add(new Text(""));
        rows.Add(BuildFacilitiesPanel(facilities));
    }

    return new Rows(rows);
}

static IRenderable BuildFacilitiesPanel(List<Facility> facilities) {
    var elevators = facilities.Where(f => f.Type == FacilityType.Elevator).ToList();
    var escalators = facilities.Where(f => f.Type == FacilityType.Escalator).ToList();

    var table = new Table {
        Border = TableBorder.Rounded,
        BorderStyle = Style.Parse(Gruvbox.Gray)
    };
    table.AddColumn(new TableColumn($"[bold {Gruvbox.Blue}]Facilities[/]").LeftAligned());
    table.AddColumn(new TableColumn($"[{Gruvbox.Blue}]Status[/]").Centered());

    var elevatorsActive = elevators.Count(f => f.State == FacilityState.Active);
    var escalatorsActive = escalators.Count(f => f.State == FacilityState.Active);

    if (elevators.Count > 0) {
        var color = elevatorsActive == elevators.Count ? Gruvbox.Green :
                    elevatorsActive == 0 ? Gruvbox.Red : Gruvbox.Yellow;
        table.AddRow(
            $"[{Gruvbox.Fg}]Elevators[/]",
            $"[{color}]{elevatorsActive}/{elevators.Count}[/]"
        );
    }

    if (escalators.Count > 0) {
        var color = escalatorsActive == escalators.Count ? Gruvbox.Green :
                    escalatorsActive == 0 ? Gruvbox.Red : Gruvbox.Yellow;
        table.AddRow(
            $"[{Gruvbox.Fg}]Escalators[/]",
            $"[{color}]{escalatorsActive}/{escalators.Count}[/]"
        );
    }

    var inactiveFacilities = facilities.Where(f => f.State != FacilityState.Active).ToList();
    if (inactiveFacilities.Count > 0) {
        table.AddEmptyRow();
        table.AddRow($"[{Gruvbox.Red}]Out of service:[/]", "");
        foreach (var facility in inactiveFacilities.Take(5)) {
            var typeIcon = facility.Type == FacilityType.Elevator ? "\u21c5" : "\u21f5";
            var desc = facility.Description ?? $"#{facility.EquipmentNumber}";
            if (desc.Length > 25) desc = desc[..22] + "...";
            table.AddRow($"[{Gruvbox.Gray}]{typeIcon} {Markup.Escape(desc)}[/]", "");
        }
        if (inactiveFacilities.Count > 5) {
            table.AddRow($"[{Gruvbox.Gray}]... and {inactiveFacilities.Count - 5} more[/]", "");
        }
    }

    return table;
}

static string FormatCategory(StationCategory category) {
    var (color, description) = category switch {
        StationCategory.Category1 => (Gruvbox.Green, "Major Hub"),
        StationCategory.Category2 => (Gruvbox.Aqua, "Large Station"),
        StationCategory.Category3 => (Gruvbox.Blue, "Important Regional"),
        StationCategory.Category4 => (Gruvbox.Fg, "Regional Station"),
        StationCategory.Category5 => (Gruvbox.Gray, "Standard Station"),
        StationCategory.Category6 => (Gruvbox.Gray, "Small Station"),
        StationCategory.Category7 => (Gruvbox.Gray, "Minor Stop"),
        _ => (Gruvbox.Yellow, "Unknown")
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

    return $"[bold {color}]{categoryNum}[/] [{Gruvbox.Gray}]({description})[/]";
}

static void AddServiceRow(Table table, string name, bool available) {
    var icon = available ? $"[{Gruvbox.Green}]Yes[/]" : $"[{Gruvbox.Red}]No[/]";
    table.AddRow($"[{Gruvbox.Fg}]{name}[/]", icon);
}

static void RenderPositionBar(int index, int total) {
    var segments = new StringBuilder();
    for (var i = 0; i < total; i++) {
        if (i == index) {
            segments.Append($"[{Gruvbox.Blue}]\u2588[/]");
        } else {
            segments.Append($"[{Gruvbox.Gray}]\u2591[/]");
        }
    }
    AnsiConsole.MarkupLine(segments.ToString());
}

static void RenderNavigation() {
    AnsiConsole.WriteLine();
    var rule = new Rule() {
        Style = Style.Parse(Gruvbox.Gray)
    };
    AnsiConsole.Write(rule);
    AnsiConsole.MarkupLine($"[{Gruvbox.Gray}]Navigation: [{Gruvbox.Fg}]\u2190[/] Previous | [{Gruvbox.Fg}]\u2192[/] Next | [{Gruvbox.Fg}]Home[/] First | [{Gruvbox.Fg}]End[/] Last | [{Gruvbox.Fg}]Q/Esc[/] Quit[/]");
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
