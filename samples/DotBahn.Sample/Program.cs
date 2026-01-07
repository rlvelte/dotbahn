using DotBahn.Cache;
using DotBahn.Cache.Configuration;
using DotBahn.Core.Client;
using DotBahn.Core.Token;
using DotBahn.TimetableApi.Client;
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

        var config = new BaseClientConfiguration {
            BaseUrl = "https://api.deutschebahn.com/timetables/v1",
            ClientId = args[0],
            ClientSecret = args[1],
            CacheOptions = new CacheOptions {
                DefaultExpiration = TimeSpan.FromSeconds(30),
                PathExpirations = new Dictionary<string, TimeSpan> {
                    { "/station/", TimeSpan.FromHours(24) }
                }
            }
        };

        using var httpClient = new HttpClient();
        var tokenService = new TokenService(config, httpClient);
        
        // Initialize Cache
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new InMemoryCache(memoryCache);
        
        using var client = new TimetableApiClient(config, tokenService, httpClient, cache);

        try {
            Console.WriteLine("1. Request (API): Frankfurt Hbf...");
            var timetable = await client.GetFullChangesAsync("8000105");
            Console.WriteLine($"   Station: {timetable.Station}, Stops: {timetable.Stops.Count}");

            Console.WriteLine("\n2. Request (Cache): Frankfurt Hbf...");
            var timetableCached = await client.GetFullChangesAsync("8000105");
            Console.WriteLine($"   Station: {timetableCached.Station}, Stops: {timetableCached.Stops.Count}");

            Console.WriteLine("\n3. Station Search (Long-term Cache):");
            var stations = await client.SearchStationsAsync("Berlin");
            Console.WriteLine($"   Found {stations.Count} stations.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}