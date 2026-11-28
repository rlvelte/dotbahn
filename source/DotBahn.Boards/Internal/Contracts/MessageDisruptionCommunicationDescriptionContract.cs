using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a disruption-communication description.
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessageDisruptionCommunicationDescriptionContract {
    /// <summary>
    /// Gets the attachments.
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<DisruptionCommunicationAttachmentContract>? Attachments { get; init; }

    /// <summary>
    /// Gets the images.
    /// </summary>
    [JsonPropertyName("images")]
    public List<DisruptionCommunicationImageContract>? Images { get; init; }

    /// <summary>
    /// Gets the language.
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the links.
    /// </summary>
    [JsonPropertyName("links")]
    public List<DisruptionCommunicationLinkContract>? Links { get; init; }

    /// <summary>
    /// Gets the text.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Gets the short text.
    /// </summary>
    [JsonPropertyName("textShort")]
    public string? TextShort { get; init; }
}
