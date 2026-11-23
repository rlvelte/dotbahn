using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a journey attribute multi-language
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessageAttributeMultiLanguageContract {
    /// <summary>
    /// Gets the code
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Gets the descriptions
    /// </summary>
    [JsonPropertyName("descriptions")]
    public List<MessageAttributeDescriptionContract>? Descriptions { get; init; }

    /// <summary>
    /// Gets the display priority
    /// </summary>
    [JsonPropertyName("displayPriority")]
    public int? DisplayPriority { get; init; }

    /// <summary>
    /// Gets the display priority detail
    /// </summary>
    [JsonPropertyName("displayPriorityDetail")]
    public int? DisplayPriorityDetail { get; init; }

    /// <summary>
    /// Gets the message identifier
    /// </summary>
    [JsonPropertyName("messageID")]
    public int MessageId { get; init; }
}
