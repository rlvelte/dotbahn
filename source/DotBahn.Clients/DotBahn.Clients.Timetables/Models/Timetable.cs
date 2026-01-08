namespace DotBahn.Clients.Timetables.Models;

/// <summary>
/// Represents a timetable containing train stops for a specific station.
/// </summary>
public record Timetable {
    /// <summary>
    /// Gets the name or identifier of the station for which the timetable is provided.
    /// </summary>
    public string Station { get; init; } = string.Empty;

    /// <summary>
    /// Gets the list of train stops included in this timetable.
    /// </summary>
    public List<TrainStop> Stops { get; init; } = [];

    /// <summary>
    /// Gets the timestamp when the timetable was last updated.
    /// </summary>
    public DateTime LastUpdated { get; init; } = DateTime.Now;
}