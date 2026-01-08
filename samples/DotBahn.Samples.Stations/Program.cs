using DotBahn.Clients.Stations;
using DotBahn.Clients.Stations.Client;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Enumerations;
using DotBahn.Modules.RequestCache;
using DotBahn.Modules.RequestCache.Enumerations;
using Microsoft.Extensions.DependencyInjection;

if (args.Length < 2) {
    Console.WriteLine("Usage: DotBahn.Sample.Stations <ClientId> <ClientSecret>");
    return;
}

var clientId = args[0];
var clientSecret = args[1];

var services = new ServiceCollection();
services.AddLogging();

// Add Authorization
services.AddAuthorizationProvider((_, opt) => {
    opt.ProviderType = AuthProviderType.ApiKey; 
    opt.ClientId = clientId;
    opt.ClientSecret = clientSecret;
});

// Add Cache
services.AddRequestCacheProvider((_, opt) => {
    opt.ProviderType = CacheProviderType.InMemory; 
    opt.DefaultExpiration = TimeSpan.FromSeconds(30); 
});

// Add Stations Client
services.AddDotBahnStations((_, opt) => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/");
});

var serviceProvider = services.BuildServiceProvider();


// Use the API
var client = serviceProvider.GetRequiredService<StationsClient>();

var stations = await client.GetStationsAsync("Berlin*");
Console.WriteLine($"Found {stations.Count} stations.");
        
foreach (var s in stations.Take(3)) {
    Console.WriteLine($"- {s.Name}");
}