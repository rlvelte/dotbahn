using DotDB.Models.Additional;
using DotDB.Models.Enumerations;

namespace DotDB.Models;

/// <summary>
/// Managed event information (arrival or departure)
/// </summary>
public class EventInfo
{
    public TimeInfo Time { get; set; }              // Time information with delay
    public PlatformInfo Platform { get; set; }      // Platform information
    public PathInfo Path { get; set; }              // Route/path information
    public EventStatus Status { get; set; }         // Event status
    public bool IsCancelled { get; set; }           // Quick check if cancelled
    public bool IsHidden { get; set; }              // Whether this event is hidden
    public string Line { get; set; }                // Line designation
    public DistantEndpoint DistantEndpoint { get; set; } // Distant destination/origin
}