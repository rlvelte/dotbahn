using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a station name multi-language.
/// </summary>
[ExcludeFromCodeCoverage]
internal record StationNameMultiLanguageContract {
    /// <summary>
    /// Gets the language.
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
