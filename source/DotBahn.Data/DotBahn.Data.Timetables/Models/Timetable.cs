namespace DotBahn.Data.Timetables.Models;

/// <summary>
/// A timetable containing stops and messages for a station.
/// </summary>
public class Timetable {
    /// <summary>
    /// Station name.
    /// </summary>
    public required string Station { get; init; }

    /// <summary>
    /// List of stops (arrivals and departures) at this station.
    /// </summary>
    public IEnumerable<TimetableStop> Stops { get; init; } = [];

    /// <summary>
    /// Station-level messages (disruptions, announcements).
    /// </summary>
    public IEnumerable<TimetableMessage> Messages { get; init; } = [];
}
