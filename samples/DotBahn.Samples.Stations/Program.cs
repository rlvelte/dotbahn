using DotBahn.Clients.Stations;
using DotBahn.Clients.Stations.Client;
using DotBahn.Clients.Stations.Models;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length < 2) {
    Console.WriteLine("Usage: DotBahn.Sample.Stations <ClientId> <ClientSecret>");
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
    opt.DefaultExpiration = TimeSpan.FromSeconds(10);
});

// Add Stations Client
services.AddDotBahnStations((_, opt) => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/");
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<StationsClient>();

var t = await client.GetStationsAsync(new StationsQuery().WithName("Nürnberg Hbf"));
foreach (var s in t.Stations) {
    Console.WriteLine($"- {s.Name}");
}

Thread.Sleep(5000);
var t2 = await client.GetStationsAsync(new StationsQuery().WithName("Nürnberg Hbf"));
foreach (var s in t2.Stations) {
    Console.WriteLine($"- {s.Name}");
}

var t3 = await client.GetStationsAsync(new StationsQuery().WithName("Nürnberg Hbf"));
foreach (var s in t3.Stations) {
    Console.WriteLine($"- {s.Name}");
}