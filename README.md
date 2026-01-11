<img src="https://i.imgur.com/3MfLPSP.png" width=200>

# DotBahn - .NET client for various Deutsche Bahn API's
![NuGet](https://img.shields.io/nuget/v/DotBahn.Timetables?label=DotBahn.Timetables&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Stations?label=DotBahn.Stations&style=flat)
![NuGet](https://img.shields.io/nuget/v/DotBahn.Facilities?label=DotBahn.Facilities&style=flat)

DotBahn is a collection of .NET client libraries and helpers that provide convenient access to various Deutsche Bahn (DB) APIs. The project aims to make it easy to query schedules, station information, and other DB services from .NET applications.

The currently available clients cover the following APIs:
- **StaDa**: Provides information about DB stations, such as parking facilities, accessibility, and opening hours.

- **FaSta**: Provides information about the operational status of elevators and escalators at German railway stations operated by DB InfraGO AG.

- **Timetables**: Provides information about current timetable data. Endpoints are available for both the officially scheduled timetable and the real-time deviations from the schedule.

> [!IMPORTANT]
> These packages were extracted from a personal project. Ongoing development is driven by my perceived needs rather than a fixed roadmap. Contributions, improvements, and forks are very welcome.


## Table of Contents
- [Install](#install)
- [Usage](#usage)
- [Authorization](#authorization)


## Install
The easiest way to get started is to install any package you need from [GitHub Packages](https://docs.github.com/de/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry) or use the versions on [Nuget](https://www.nuget.org/):
```
dotnet add package DotBahn.Timetables
dotnet add package DotBahn.Stations
dotnet add package DotBahn.Facilities
```


## Usage
### Dependency Injection (recommended)
The packages are designed have easy integration with the `ServiceCollection`.
```
// Add Authorization
services.AddDotBahnAuthorization(opt => {
    opt.ClientId = <your-client-id>, 
    opt.ApiKey = <your-client-secret>
});

// Add Stations Client
services.AddDotBahnStations(opt => {
    opt.BaseEndpoint = new Uri("...");
});
```

You can also use the request caching system to reduce load:
```
// Add Cache
services.AddDotBahnCache(opt => {
    opt.DefaultExpiration = TimeSpan.FromSeconds(...); 
});
```

### Manual Initialization
You can also use the clients in a more conventional way by creating instances manually and providing the options yourself:
```
var options = new ClientOptions {
    BaseEndpoint = new Uri("...")
};

var auth = new AuthorizationOptions {
    ClientId = <your-client-id>, 
    ApiKey = <your-client-secret>
};

var client = new StationsClient(opt, auth);
```

> [!NOTE]
> Additional sample projects demonstrating how to use the clients can be found in the `samples/` directory.


## Authorization
You need a Deutsche Bahn API Key to use these packages. Information on how to get started is available [here](https://developers.deutschebahn.com/db-api-marketplace/apis/start).