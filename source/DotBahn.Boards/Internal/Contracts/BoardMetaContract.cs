using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a board meta
/// </summary>
[ExcludeFromCodeCoverage]
internal record BoardMetaContract {
    /// <summary>
    /// Gets the considered stop places
    /// </summary>
    [JsonPropertyName("consideredStopPlaces")]
    public List<string>? ConsideredStopPlaces { get; init; }

    /// <summary>
    /// Gets the truncated transport types
    /// </summary>
    [JsonPropertyName("truncatedTransportTypes")]
    public List<string>? TruncatedTransportTypes { get; init; }
}
