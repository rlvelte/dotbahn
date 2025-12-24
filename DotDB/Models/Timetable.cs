namespace DotDB.Models;

/// <summary>
/// Managed timetable response
/// </summary>
public class Timetable
{
    public string Station { get; set; }               // Station name
    public List<TrainStop> Stops { get; set; }        // All train stops
    public DateTime LastUpdated { get; set; }         // When this data was fetched

    /// <summary>
    /// Gets all delayed trains
    /// </summary>
    public List<TrainStop> GetDelayedTrains() => 
        Stops.Where(s => s.HasDelay).ToList();

    /// <summary>
    /// Gets all cancelled trains
    /// </summary>
    public List<TrainStop> GetCancelledTrains() => 
        Stops.Where(s => s.IsCancelled).ToList();

    /// <summary>
    /// Gets all trains with platform changes
    /// </summary>
    public List<TrainStop> GetPlatformChanges() => 
        Stops.Where(s => s.HasPlatformChange).ToList();
}