using System.Text.Json.Serialization;

namespace DotBahn.Clients.Stations.Contracts;

/// <summary>
/// Raw structure for an EVA number.
/// </summary>
public record EvaNumberContract {
    /// <summary>
    /// Gets the EVA number.
    /// </summary>
    [JsonPropertyName("number")]
    public long Number { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is the main EVA number.
    /// </summary>
    [JsonPropertyName("isMain")]
    public bool IsMain { get; init; }

    /// <summary>
    /// Gets the geographic coordinates.
    /// </summary>
    [JsonPropertyName("geographicCoordinates")]
    public GeographicCoordinatesContract? GeographicCoordinates { get; init; }
}