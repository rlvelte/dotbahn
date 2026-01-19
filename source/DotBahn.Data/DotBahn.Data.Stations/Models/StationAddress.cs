namespace DotBahn.Data.Stations.Models;

/// <summary>
/// Mailing address of a railway station.
/// </summary>
public class StationAddress {
    /// <summary>
    /// Street name and house number.
    /// </summary>
    public required string Street { get; init; }

    /// <summary>
    /// Postal code (ZIP code).
    /// </summary>
    public required string ZipCode { get; init; }

    /// <summary>
    /// City or municipality name.
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Formatted single-line address.
    /// </summary>
    /// <example>"Bahnhofstraße 1, 12345 Musterstadt"</example>
    public string DisplayAddress => $"{Street}, {ZipCode} {City}";
}
