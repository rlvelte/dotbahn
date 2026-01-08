using DotBahn.Modules.Auth;
using DotBahn.Modules.Auth.Enumerations;
using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Enumerations;
using DotBahn.TimetableApi;
using DotBahn.TimetableApi.Client;
using Microsoft.Extensions.DependencyInjection;

if (args.Length < 2) {
    Console.WriteLine("Usage: DotBahn.Sample.TimetableApi <ClientId> <ClientSecret>");
    return;
}

var clientId = args[0];
var clientSecret = args[1];

var services = new ServiceCollection();
services.AddLogging();

// Add Authorization
services.AddAuthorizationProvider((_, opt) => {
    opt.ProviderType = AuthProviderType.Token; 
    opt.ClientId = clientId;
    opt.ClientSecret = clientSecret;
});

// Add Cache
services.AddCacheProvider((_, opt) => {
    opt.ProviderType = CacheProviderType.InMemory; 
    opt.DefaultExpiration = TimeSpan.FromMinutes(5); 
});

// Add Timetable API Client
services.AddDotBahnTimetableApi((_, opt) => {
    opt.BaseEndpoint = new Uri("https://api.deutschebahn.com/timetables/v1");
});

var serviceProvider = services.BuildServiceProvider();


// Use the API
var client = serviceProvider.GetRequiredService<TimetableApiClient>();

Console.WriteLine("Searching for stations matching 'Berlin'...");
var stations = await client.SearchStationsAsync("Berlin");

foreach (var station in stations.Take(5)) {
    Console.WriteLine($"Found: {station.Name} (EVA: {station.Eva})");
    
    if (station == stations.First()) {
        Console.WriteLine($"\nFetching planned timetable for {station.Name}...");
        var timetable = await client.GetPlannedTimetableAsync(station.Eva, DateTime.Now);
        Console.WriteLine($"Found {timetable.Stops.Count} stops.");
        
        foreach (var stop in timetable.Stops.Take(3)) {
            var platform = stop.Arrival?.Platform.Current ?? stop.Departure?.Platform.Current ?? "N/A";
            Console.WriteLine($"- Train: {stop.Train.Category} {stop.Train.Number}, Platform: {platform}");
        }
    }
}