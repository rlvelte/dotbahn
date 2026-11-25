using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a station multi-language.
/// </summary>
[ExcludeFromCodeCoverage]
internal record StationMultiLanguageContract {
    /// <summary>
    /// Gets the localized names.
    /// </summary>
    [JsonPropertyName("localizedNames")]
    public List<StationNameMultiLanguageContract>? LocalizedNames { get; init; }

    /// <summary>
    /// Gets the STADA identifier.
    /// </summary>
    [JsonPropertyName("stada")]
    public string Stada { get; init; } = string.Empty;
}
