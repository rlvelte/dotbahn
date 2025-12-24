using DotDB.Client;
using DotDB.Client.Configuration;

namespace DotDB;

/// <summary>
/// Example usage of the API client
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new DotBahnConfiguration
        {
            BaseUrl = "https://api.deutschebahn.com/timetables/v1",
            ClientId = "YOUR_CLIENT_ID",
            ClientSecret = "YOUR_CLIENT_SECRET"
        };

        using var client = new TimetableApiClient(config);

        try
        {
            // Get full changes for Frankfurt Hbf (EVA: 8000105)
            var timetable = await client.GetFullChangesAsync("8000105");

            Console.WriteLine($"Timetable for: {timetable.Station}");
            Console.WriteLine($"Total stops: {timetable.Stops.Count}");
            Console.WriteLine($"Delayed trains: {timetable.GetDelayedTrains().Count}");
            Console.WriteLine($"Cancelled trains: {timetable.GetCancelledTrains().Count}");

            // Show delayed trains
            Console.WriteLine("\nDelayed Trains:");
            foreach (var stop in timetable.GetDelayedTrains())
            {
                var delay = stop.Departure?.Time.Delay ?? stop.Arrival?.Time.Delay ?? 0;
                Console.WriteLine(
                    $"{stop.Train.Category} {stop.Train.Number}: +{delay} minutes"
                );
            }

            // Show platform changes
            Console.WriteLine("\nPlatform Changes:");
            foreach (var stop in timetable.GetPlatformChanges())
            {
                var platform = stop.Departure?.Platform ?? stop.Arrival?.Platform;
                Console.WriteLine(
                    $"{stop.Train.Category} {stop.Train.Number}: " +
                    $"{platform.Planned} → {platform.Changed}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching timetable: {ex.Message}");
        }
    }
}