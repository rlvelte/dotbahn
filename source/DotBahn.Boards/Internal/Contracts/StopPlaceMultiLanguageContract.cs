using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a stop-place multi-language.
/// </summary>
[ExcludeFromCodeCoverage]
internal record StopPlaceMultiLanguageContract {
    /// <summary>
    /// Gets the EVA number.
    /// </summary>
    [JsonPropertyName("evaNumber")]
    public string EvaNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the IFOPT.
    /// </summary>
    [JsonPropertyName("ifopt")]
    public string? Ifopt { get; init; }

    /// <summary>
    /// Gets the localized names.
    /// </summary>
    [JsonPropertyName("localizedNames")]
    public List<StopPlaceNameMultiLanguageContract>? LocalizedNames { get; init; }

    /// <summary>
    /// Gets the STADA identifier.
    /// </summary>
    [JsonPropertyName("stada")]
    public string? Stada { get; init; }
}
