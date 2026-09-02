namespace DotBahn.Common.Models;

/// <summary>
/// Geographic coordinates representing the location of an entity
/// </summary>
public readonly record struct Coordinates {
    /// <summary>
    /// Longitude in decimal degrees (east-west position).
    /// </summary>
    public required double Longitude { get; init; }

    /// <summary>
    /// Latitude in decimal degrees (north-south position).
    /// </summary>
    public required double Latitude { get; init; }
}
