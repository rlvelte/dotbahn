namespace DotDB.Models;

/// <summary>
/// Managed train stop at station
/// </summary>
public class TrainStop
{
    public string Id { get; set; }                      // Unique stop ID
    public StationInfo Station { get; set; }            // Station information
    public TrainInfo Train { get; set; }                // Train information
    public EventInfo Arrival { get; set; }              // Arrival information
    public EventInfo Departure { get; set; }            // Departure information
    public List<Message> Messages { get; set; }         // All messages for this stop

    // Computed properties
    public bool HasDelay { get; set; }                  // Quick check if any delay
    public bool HasPlatformChange { get; set; }         // Quick check if platform changed
    public bool HasRouteChange { get; set; }            // Quick check if route changed
    public bool IsCancelled { get; set; }               // Quick check if cancelled
}