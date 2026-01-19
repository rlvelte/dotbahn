namespace DotBahn.Data.Shared.Models;

/// <summary>
/// Geographic coordinates representing the location of a entity.
/// </summary>
public class Coordinates {
    /// <summary>
    /// Longitude in decimal degrees (east-west position).
    /// Positive values indicate east of the Prime Meridian.
    /// </summary>
    public required double Longitude { get; init; }

    /// <summary>
    /// Latitude in decimal degrees (north-south position).
    /// Positive values indicate north of the Equator.
    /// </summary>
    public required double Latitude { get; init; }
}
