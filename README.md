<img src="docs/images/logo-256x256.png" width=128 alt="logo">

# DotBahn - .NET Client for Deutsche Bahn APIs
![NuGet](https://img.shields.io/nuget/v/DotBahn.Timetables?label=DotBahn.Timetables&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Stations?label=DotBahn.Stations&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Facilities?label=DotBahn.Facilities&style=flat)
[![Docs](https://img.shields.io/badge/docs-github_pages-blue?style=flat)](https://rlvelte.github.io/dotbahn/)

DotBahn is a collection of .NET client libraries for accessing Deutsche Bahn (DB) APIs. Query train schedules, station details, and facility status directly from your application.

Available clients:
- **Stations (StaDa)**: Station data including parking, accessibility, and opening hours.
- **Facilities (FaSta)**: Real-time operational status of elevators and escalators at stations.
- **Timetables**: Scheduled departures and arrivals with real-time delay and platform change information.

## Table of Contents
- [Install](#install)
- [Usage](#usage)
- [API Reference](https://rlvelte.github.io/dotbahn/index.html)
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
All packages integrate seamlessly with `ServiceCollection`. Each client comes with a default endpoint, override only for custom proxies:

```csharp
// Register clients
services.AddDotBahnStations();
services.AddDotBahnTimetables();
services.AddDotBahnFacilities();

// Configure authorization (required for API access)
services.AddDotBahnAuthorization(opt => {
    opt.ClientId = clientId;
    opt.ApiKey = clientSecret;
});

// ---

// Optional: override default endpoints
services.AddDotBahnStations(opt => {
    opt.BaseEndpoint = new Uri("https://custom-proxy.example.com/stada");
});
```

Enable request caching to reduce API calls via the integrated cacheing:

```csharp
services.AddDotBahnCache(opt => {
    opt.DefaultExpiration = TimeSpan.FromSeconds(60);
});
```

### Manual Initialization
Create client instances directly without dependency injection. The caller provides the `HttpClient` lifecycle:

```csharp
using var http = new HttpClient();
using var client = new StationClient(http,
    new ClientOptions {
        BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/")
    },
    new AuthorizationOptions {
        ClientId = clientId,
        ApiKey = clientSecret
    });
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
