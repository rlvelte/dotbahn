using DotBahn.Core.Client;
using DotBahn.Core.Token;
using DotBahn.TimetableApi.Client;

namespace DotBahn.Sample;

/// <summary>
/// Example usage of the API client
/// </summary>
public static class Program {
    public static async Task Main(string[] args) {
        var config = new BaseClientConfiguration() {
            BaseUrl = "https://api.deutschebahn.com/timetables/v1",
            ClientId = "YOUR_CLIENT_ID",
            ClientSecret = "YOUR_CLIENT_SECRET"
        };

        using var httpClient = new HttpClient();
        var tokenService = new TokenService(config, httpClient);
        using var client = new TimetableApiClient(config, tokenService, httpClient);

        try {
            // Get full changes for Frankfurt Hbf (EVA: 8000105)
            var timetable = await client.GetFullChangesAsync("8000105");

            Console.WriteLine($"Timetable for: {timetable.Station}");
            Console.WriteLine($"Total stops: {timetable.Stops.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching timetable: {ex.Message}");
        }
    }
}