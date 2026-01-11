using System.Text.Json;
using DotBahn.Clients.Timetables;
using DotBahn.Clients.Timetables.Client;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length < 2) {
    Console.WriteLine("Usage: DotBahn.Samples.Timetables <ClientId> <ClientSecret>");
    return;
}

var clientId = args[0];
var clientSecret = args[1];

var services = new ServiceCollection();

// Add Logging
services.AddLogging(builder => {
    builder.SetMinimumLevel(LogLevel.Debug);
});

// Add Authorization
services.AddAuthorizationProvider((_, opt) => {
    opt.ClientId = clientId;
    opt.ClientSecret = clientSecret;
});

// Add Cache
services.AddCacheProvider((_, opt) => {
    opt.DefaultExpiration = TimeSpan.FromSeconds(30); 
});

// Add Timetables Client
services.AddDotBahnTimetables((_, opt) => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1");
});

// Usage
var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<TimetablesClient>();

var timetable = await client.GetPlannedTimetableAsync(8000261, DateTime.Now);

Console.WriteLine($"""
                  {timetable.Station.ToUpper()}
                  ==========================================================
                  """);
foreach (var stop in timetable.Stops) {
    Console.WriteLine(JsonSerializer.Serialize(stop));
}
