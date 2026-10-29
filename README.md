<img src="docs/images/logo-256x256.png" width=128 alt="logo">

# DotBahn - .NET Client for Deutsche Bahn APIs
![NuGet](https://img.shields.io/nuget/v/DotBahn.Common?label=DotBahn.Common&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Telemetry?label=DotBahn.Telemetry&style=flat)

![NuGet](https://img.shields.io/nuget/v/DotBahn.Timetables?label=DotBahn.Timetables&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Stations?label=DotBahn.Stations&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Facilities?label=DotBahn.Facilities&style=flat)


DotBahn is a collection of unofficial .NET client packages for accessing Deutsche Bahn (DB) APIs. Query train schedules, station details, and facility status directly in your application. It also provides convenience features like timetable merging, fluent queries and some additional properties. All libraries are AOT compatible and trimmable.

> [!IMPORTANT]
> If you are just here to use the departure monitor as a standalone tool on your system, there is also a packaged RPM on the [Open Build Service](https://build.opensuse.org/package/show/home:rvelte/dbpeek) for that.

| API                    | Description |
|------------------------|-------------|
| **Stations (StaDa)**   | Station data including parking, accessibility, and opening hours. |
| **Facilities (FaSta)** | Real-time operational status of elevators and escalators at stations. |
| **Timetables**         | Scheduled departures and arrivals with real-time delay and platform change information. |

## Table of Contents
- [Install](#install)
- [Usage](#usage)
- [Authorization](#authorization)
- [Samples](#samples)
- [API Reference](https://rlvelte.github.io/dotbahn/index.html)


## Install
Install the packages you need from [NuGet](https://www.nuget.org/) or [GitHub Packages](https://docs.github.com/de/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry):
```bash
dotnet add package DotBahn.Timetables
dotnet add package DotBahn.Stations
dotnet add package DotBahn.Facilities

# Optional; see below for further details.
dotnet add package DotBahn.Telemetry
```

> [!NOTE]
> All client packages depend on **DotBahn.Common**, which provides cross-cutting abstractions such as authorization, base client infrastructure, and shared models. It is installed automatically as a transitive dependency.


## Usage
### Dependency Injection (Recommended)
All packages integrate seamlessly with `IServiceCollection`. Each client comes with a default endpoint, override only for custom proxies:

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

...

// Optional: override default endpoints
services.AddDotBahnStations(opt => {
    opt.BaseEndpoint = new Uri("https://custom-proxy.example.com/stada");
});
```

### Manual Initialization
Create client instances directly without dependency injection. Each client creates and owns its own `HttpClient`:

```csharp
using var client = new StationClient(
    new ClientOptions {
        BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/")
    },
    new AuthorizationOptions {
        ClientId = clientId,
        ApiKey = clientSecret
    });
```

### Telemetry
All clients emit OpenTelemetry-compatible traces and metrics using the built-in `ActivitySource` and `Meter` APIs. The client packages themselves have no OpenTelemetry dependency. Enable collection with the **DotBahn.Telemetry** package, which registers the source and meter with your OpenTelemetry providers:

```csharp
// OpenTelemetry configuration
services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddOtlpExporter())
    .WithMetrics(metrics => metrics.AddOtlpExporter());

// Add dotbahn specific activites
services.AddDotBahnTelemetry();
```

## Authorization
A Deutsche Bahn API key is required. Register and obtain your credentials at the [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/start).

> [!WARNING]
> These packages only consume the credentials you provide for API authentication. They do not create, register, or manage credentials on your behalf.

## Samples
Both samples take credentials via `--client` / `--secret` or, more conveniently, the `DOTBAHN_CLIENT` / `DOTBAHN_SECRET` environment variables.

### TimetablePeek (`dbpeek`)
Terminal departures board for DB timetable services. Shows customer-facing train names (service line or trip number), planned and actual times, delays, platforms, destinations, and via stops. Optional category filter, upstream time window and machine-readable JSON output.

```bash
dotnet run --project samples/DotBahn.Samples.TimetablePeek -- <EVA> [options]
```

<img src="https://i.imgur.com/Z3fKMo5.png" width=500>

### StationBrowser (`dbstation`)
Interactive terminal application for exploring DB station details. Search by name and navigate results with arrow keys. Shows station category, identifiers (EVA/RIL100), address, coordinates, regional area, available services, and real-time elevator/escalator status.

```bash
dotnet run --project samples/DotBahn.Samples.StationBrowser -- <SearchName> [options]
```

<img src="https://i.imgur.com/XWwBVr2.png" width=500>
