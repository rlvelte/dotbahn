namespace DotBahn.Timetables.Models;

/// <summary>
/// Represents information about a railway station.
/// </summary>
public record StationInfo {
    /// <summary>
    /// Gets the EVA station number (a unique identifier for stations in the German rail system).
    /// </summary>
    public string Eva { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the station.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}