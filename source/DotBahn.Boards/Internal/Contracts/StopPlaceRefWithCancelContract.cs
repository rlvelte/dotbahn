using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a stop-place reference with cancellation.
/// </summary>
[ExcludeFromCodeCoverage]
internal record StopPlaceRefWithCancelContract {
    /// <summary>
    /// Gets a value indicating whether this is cancelled.
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("cancelled")]
    public bool Cancelled { get; init; }

    /// <summary>
    /// Gets the EVA number.
    /// </summary>
    [JsonPropertyName("evaNumber")]
    public string EvaNumber { get; init; } = string.Empty;
}
