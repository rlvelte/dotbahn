using System.Text;
using DotBahn.Clients.Stations;
using DotBahn.Clients.Stations.Client;
using DotBahn.Clients.Stations.Query;
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

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<StationsClient>();

List<Station> stations = [];
await AnsiConsole.Status()
                 .Spinner(Spinner.Known.Dots)
                 .StartAsync($"Searching for stations matching '{searchName}'...", async _ => {
                     var query = new StationsQuery().WithNames(searchName);
                     var result = await client.GetStationsAsync(query);
                     stations.AddRange(result);
                 });

if (stations.Count == 0) {
    AnsiConsole.MarkupLine($"[{Gruvbox.Yellow}]No stations found matching '{Markup.Escape(searchName)}'[/]");
    return 1;
}

AnsiConsole.MarkupLine($"[{Gruvbox.Green}]Found {stations.Count} station(s)[/]");

var currentIndex = 0;
while (true) {
    AnsiConsole.Clear();
    RenderStation(stations[currentIndex], currentIndex, stations.Count);
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

static void RenderStation(Station station, int index, int total) {
    var rule = new Rule($"[bold {Gruvbox.Blue}]{Markup.Escape(station.Name)}[/]") {
        Justification = Justify.Center,
        Style = Style.Parse(Gruvbox.Blue)
    };

    AnsiConsole.Write(rule);
    RenderPositionBar(index, total);
    AnsiConsole.WriteLine();

    var leftPanel = BuildStationInfo(station);
    var rightPanel = BuildServicesPanel(station.Services);

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

static IRenderable BuildServicesPanel(StationServices services) {
    var table = new Table {
        Border = TableBorder.Rounded,
        BorderStyle = Style.Parse(Gruvbox.Gray)
    };
    table.AddColumn(new TableColumn($"[bold {Gruvbox.Blue}]Service[/]").LeftAligned());
    table.AddColumn(new TableColumn($"[{Gruvbox.Blue}]?[/]").Centered());

    AddServiceRow(table, "Parking", services.HasParking);
    AddServiceRow(table, "Bicycle Parking", services.HasBicycleParking);
    AddServiceRow(table, "Public Facilities", services.HasPublicFacilities);
    AddServiceRow(table, "Locker System", services.HasLockerSystem);
    AddServiceRow(table, "Step-Free Access", services.HasStepFreeAccess);
    AddServiceRow(table, "Stepless Access", services.HasSteplessAccess);
    AddServiceRow(table, "Travel Center", services.HasTravelCenter);
    AddServiceRow(table, "Travel Necessities", services.HasTravelNecessities);
    AddServiceRow(table, "Mobility Service", services.HasMobilityService);
    AddServiceRow(table, "WiFi", services.HasWiFi);

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
