using DotBahn.Cache;
using DotBahn.Cache.Configuration;
using DotBahn.Core.Client;
using DotBahn.Core.Token;
using DotBahn.TimetableApi;
using DotBahn.TimetableApi.Client;
using DotBahn.TimetableApi.Transformers;
using Microsoft.Extensions.Caching.Memory;

namespace DotBahn.Sample;

/// <summary>
/// Example usage of the API client with caching
/// </summary>
public static class Program {
    public static async Task Main(string[] args) {
        if (args.Length < 2) {
            Console.WriteLine("Usage: DotBahn.Sample <ClientId> <ClientSecret>");
            return;
        }

        var options = new DotBahnTimetableOptions {
            BaseUri = new Uri("https://api.deutschebahn.com/timetables/v1"),
            ClientId = args[0],
            ClientSecret = args[1],
            CacheOptions = new CacheOptions {
                DefaultExpiration = TimeSpan.FromSeconds(30)
            }
        };

        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(options);

        using var httpClient = new HttpClient();
        var tokenService = new TokenService(optionsWrapper, httpClient);
        
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new InMemoryCache(memoryCache);

        // Manually creating the transformer hierarchy (or use DI in a real app)
        var transformer = new TimetableTransformer(
            new StopTransformer(new EventTransformer(), new MessageTransformer())
        );
        var stationTransformer = new StationTransformer();
        
        using var client = new TimetableApiClient(optionsWrapper, tokenService, httpClient, transformer, stationTransformer, cache);

        try {
            Console.WriteLine("1. Request (API): Frankfurt Hbf...");
            var timetable = await client.GetFullChangesAsync("8000105");
            Console.WriteLine($"   Station: {timetable.Station}, Stops: {timetable.Stops.Count}");

            Console.WriteLine("\n2. Request (Cache): Frankfurt Hbf...");
            var timetableCached = await client.GetFullChangesAsync("8000105");
            Console.WriteLine($"   Station: {timetableCached.Station}, Stops: {timetableCached.Stops.Count}");

            Console.WriteLine("\n3. Station Search:");
            var stations = await client.SearchStationsAsync("Berlin");
            Console.WriteLine($"   Found {stations.Count} stations.");
        }
        catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}