<img src="https://imgur.com/a/t5njGdD.png" width=200>

# DotBahn - .NET client for various Deutsche Bahn API's

DotBahn is a collection of .NET client libraries and helpers that provide convenient access to various Deutsche Bahn (DB) APIs. The project aims to make it easy to query schedules, station information, and other DB services from .NET applications.

The currently available clients cover the following APIs:
- **StaDa**: Provides information about DB stations, such as parking facilities, accessibility, and opening hours.

- **FaSta**: Provides information about the operational status of elevators and escalators at German railway stations operated by DB InfraGO AG.

- **Timetables**: Provides information about current timetable data. Endpoints are available for both the officially scheduled timetable and the real-time deviations from the schedule.

> [!IMPORTANT]
> These packages were extracted from a personal project. Ongoing development is driven by my perceived needs rather than a fixed roadmap. Contributions, improvements, and forks are very welcome.

## Install
The easiest way to get started is to install any package you need from [GitHub Packages](https://docs.github.com/de/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry) or use the versions on [Nuget](https://www.nuget.org/):
```
dotnet add package Dotbahn.Timetables
dotnet add package Dotbahn.Stations
dotnet add package Dotbahn.Facilities
```

## Authorization
You need a Deutsche Bahn API Key to use these packages. Information on how to get started is available [here](https://developers.deutschebahn.com/db-api-marketplace/apis/start).