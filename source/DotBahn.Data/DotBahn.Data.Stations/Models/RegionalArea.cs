namespace DotBahn.Data.Stations.Models;

/// <summary>
/// Regional administrative area of Deutsche Bahn responsible for the station.
/// </summary>
public class RegionalArea {
    /// <summary>
    /// Unique number identifying the regional area.
    /// </summary>
    public required int Number { get; init; }

    /// <summary>
    /// Full name of the regional area.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Abbreviated name of the regional area.
    /// </summary>
    public required string ShortName { get; init; }
}
