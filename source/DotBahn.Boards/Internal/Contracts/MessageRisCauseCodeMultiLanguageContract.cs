using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a RIS cause-code multi-language
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessageRisCauseCodeMultiLanguageContract {
    /// <summary>
    /// Gets the code
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Gets the descriptions
    /// </summary>
    [JsonPropertyName("descriptions")]
    public List<MessageRisCauseCodeDescriptionContract>? Descriptions { get; init; }

    /// <summary>
    /// Gets the message identifier
    /// </summary>
    [JsonPropertyName("messageID")]
    public int MessageId { get; init; }
}
