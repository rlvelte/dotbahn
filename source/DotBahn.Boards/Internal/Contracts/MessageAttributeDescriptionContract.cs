using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a message attribute description.
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessageAttributeDescriptionContract {
    /// <summary>
    /// Gets the language.
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the text.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;
}
