using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a via stop-place reference
/// </summary>
[ExcludeFromCodeCoverage]
internal record StopPlaceRefViaContract {
    /// <summary>
    /// Gets a value indicating whether this is additional
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("additional")]
    public bool Additional { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is cancelled
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("cancelled")]
    public bool Cancelled { get; init; }

    /// <summary>
    /// Gets the display priority
    /// </summary>
    [JsonPropertyName("displayPriority")]
    public int DisplayPriority { get; init; }

    /// <summary>
    /// Gets the EVA number
    /// </summary>
    [JsonPropertyName("evaNumber")]
    public string EvaNumber { get; init; } = string.Empty;
}
