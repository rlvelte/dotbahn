using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a stop-place reference
/// </summary>
[ExcludeFromCodeCoverage]
internal record StopPlaceRefContract {
    /// <summary>
    /// Gets the EVA number
    /// </summary>
    [JsonPropertyName("evaNumber")]
    public string EvaNumber { get; init; } = string.Empty;
}
