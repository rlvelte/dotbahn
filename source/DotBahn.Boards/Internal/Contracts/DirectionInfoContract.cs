using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a direction information.
/// </summary>
[ExcludeFromCodeCoverage]
internal record DirectionInfoContract {
    /// <summary>
    /// Gets the stop places.
    /// </summary>
    [JsonPropertyName("stopPlaces")]
    public List<StopPlaceRefContract>? StopPlaces { get; init; }

    /// <summary>
    /// Gets the texts.
    /// </summary>
    [JsonPropertyName("texts")]
    public List<string>? Texts { get; init; }
}
