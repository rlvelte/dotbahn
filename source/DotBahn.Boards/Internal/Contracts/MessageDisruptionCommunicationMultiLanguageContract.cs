using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a disruption-communication multi-language
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessageDisruptionCommunicationMultiLanguageContract {
    /// <summary>
    /// Gets the cause
    /// </summary>
    [JsonPropertyName("cause")]
    public string? Cause { get; init; }

    /// <summary>
    /// Gets the descriptions
    /// </summary>
    [JsonPropertyName("descriptions")]
    public List<MessageDisruptionCommunicationDescriptionContract>? Descriptions { get; init; }

    /// <summary>
    /// Gets the display priority
    /// </summary>
    [JsonPropertyName("displayPriority")]
    public int? DisplayPriority { get; init; }

    /// <summary>
    /// Gets the disruption communication identifier
    /// </summary>
    [JsonPropertyName("disruptionCommunicationID")]
    public string DisruptionCommunicationId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the disruption identifier
    /// </summary>
    [JsonPropertyName("disruptionID")]
    public string DisruptionId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the effect
    /// </summary>
    [JsonPropertyName("effect")]
    public string Effect { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether alternatives are available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("hasAlternatives")]
    public bool? HasAlternatives { get; init; }

    /// <summary>
    /// Gets the message identifier
    /// </summary>
    [JsonPropertyName("messageID")]
    public int MessageId { get; init; }
}
