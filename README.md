<img src="https://i.imgur.com/3MfLPSP.png" width=200>

# DotBahn - .NET Client for Deutsche Bahn APIs
![NuGet](https://img.shields.io/nuget/v/DotBahn.Timetables?label=DotBahn.Timetables&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Stations?label=DotBahn.Stations&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Facilities?label=DotBahn.Facilities&style=flat)

DotBahn is a collection of .NET client libraries for accessing Deutsche Bahn (DB) APIs. Query train schedules, station details, and facility status directly from your .NET applications.

Available clients:
- **StaDa**: Station data including parking, accessibility, and opening hours.

- **FaSta**: Real-time operational status of elevators and escalators at DB InfraGO AG stations.

- **Timetables**: Scheduled departures and arrivals with real-time delay and platform change information.

> [!IMPORTANT]
> This project originated from a personal application. Development follows my own needs rather than a fixed roadmap. Contributions and forks are welcome.


## Table of Contents
- [Install](#install)
- [Usage](#usage)
- [Samples](#samples)
- [Authorization](#authorization)


## Install
Install the packages you need from [NuGet](https://www.nuget.org/) or [GitHub Packages](https://docs.github.com/de/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry):
```bash
dotnet add package DotBahn.Timetables
dotnet add package DotBahn.Stations
dotnet add package DotBahn.Facilities
```


## Usage
### Dependency Injection (Recommended)
All packages integrate seamlessly with `ServiceCollection`.

```csharp
// Add Stations/Timetables/Facilities clients
services.AddDotBahnStations(opt => {
    opt.ClientId = <your-client-id>;
    opt.ApiKey = <your-client-secret>;
    opt.BaseEndpoint = new Uri("...");
});

services.AddDotBahnTimetables(opt => {
    opt.BaseEndpoint = new Uri("...");
});

services.AddDotBahnFacilities(opt => {
    opt.BaseEndpoint = new Uri("...");
});
```

At least one client must include authorization credentials. Alternatively, configure authorization centrally:

```csharp
// Add central authorization
services.AddDotBahnAuthorization(opt => {
    opt.ClientId = clientId;
    opt.ApiKey = clientSecret;
});
```

Enable request caching to reduce API calls:

```csharp
// Add Cache
services.AddDotBahnCache(opt => {
    opt.DefaultExpiration = TimeSpan.FromSeconds(...); 
});
```

### Manual Initialization
Create client instances directly without dependency injection:
```csharp
var options = new ClientOptions {
    BaseEndpoint = new Uri("...")
};

var auth = new AuthorizationOptions {
    ClientId = <your-client-id>, 
    ApiKey = <your-client-secret>
};

var client = new StationsClient(opt, auth);
```

## Samples
### ICE Monitor
A terminal-based departure board for ICE trains at a given station. Displays train numbers, scheduled and actual departure times, platforms, destinations, and routes. Highlights delays and platform changes in real time. Refreshes automatically every 2 minutes.

```bash
dotnet run --project samples/DotBahn.Samples.IceMonitor -- <EVA> <your-client-id> <your-client-secret>
```

<img src="https://i.imgur.com/Z3fKMo5.png" width=500>

### Station Browser
An interactive terminal application for exploring DB station details. Search by name and navigate results with arrow keys. Shows station category, identifiers (EVA/RIL100), address, coordinates, regional area, available services, and real-time elevator/escalator status.

```bash
dotnet run --project samples/DotBahn.Samples.StationBrowser -- <SearchName> <your-client-id> <your-client-secret>
```

<img src="https://i.imgur.com/XWwBVr2.png" width=500>

## Authorization
A Deutsche Bahn API key is required. Register and obtain your credentials at the [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/start).