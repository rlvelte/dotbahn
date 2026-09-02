using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Stations.Internal.Contracts;

/// <summary>
/// Raw structure for an EVA number
/// </summary>
[ExcludeFromCodeCoverage]
internal record EvaNumberContract {
    /// <summary>
    /// Gets the EVA number
    /// </summary>
    [JsonPropertyName("number")]
    public long Number { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is the main EVA number
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("isMain")]
    public bool IsMain { get; init; }

    /// <summary>
    /// Gets the geographic coordinates
    /// </summary>
    [JsonPropertyName("geographicCoordinates")]
    public GeographicCoordinatesContract? GeographicCoordinates { get; init; }
}
