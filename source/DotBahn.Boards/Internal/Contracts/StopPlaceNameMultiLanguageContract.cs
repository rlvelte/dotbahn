using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a stop-place name multi-language
/// </summary>
[ExcludeFromCodeCoverage]
internal record StopPlaceNameMultiLanguageContract {
    /// <summary>
    /// Gets the language
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string? Symbol { get; init; }

    /// <summary>
    /// Gets the symbol name
    /// </summary>
    [JsonPropertyName("symbolName")]
    public string? SymbolName { get; init; }
}
