using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Stations.Internal.Contracts;

/// <summary>
/// Raw structure for geographic coordinates
/// </summary>
[ExcludeFromCodeCoverage]
internal record GeographicCoordinatesContract {
    /// <summary>
    /// Gets the type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the coordinates
    /// </summary>
    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; init; } = [];
}
