using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a note multi-language.
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessageNoteMultiLanguageContract {
    /// <summary>
    /// Gets the category.
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; init; }

    /// <summary>
    /// Gets the code.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Gets the descriptions.
    /// </summary>
    [JsonPropertyName("descriptions")]
    public List<MessageNoteDescriptionContract>? Descriptions { get; init; }

    /// <summary>
    /// Gets the display priority.
    /// </summary>
    [JsonPropertyName("displayPriority")]
    public int? DisplayPriority { get; init; }

    /// <summary>
    /// Gets the disruption communication identifier.
    /// </summary>
    [JsonPropertyName("disruptionCommunicationID")]
    public string? DisruptionCommunicationId { get; init; }

    /// <summary>
    /// Gets the disruption identifier.
    /// </summary>
    [JsonPropertyName("disruptionID")]
    public string? DisruptionId { get; init; }

    /// <summary>
    /// Gets the message identifier.
    /// </summary>
    [JsonPropertyName("messageID")]
    public int MessageId { get; init; }
}
