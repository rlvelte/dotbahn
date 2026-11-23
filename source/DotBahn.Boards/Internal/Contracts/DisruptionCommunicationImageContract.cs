using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a disruption-communication image
/// </summary>
[ExcludeFromCodeCoverage]
internal record DisruptionCommunicationImageContract {
    /// <summary>
    /// Gets the label
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; init; } = string.Empty;

    /// <summary>
    /// Gets the URL
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}
