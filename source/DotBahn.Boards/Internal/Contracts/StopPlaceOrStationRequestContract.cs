using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a stop-place or station request
/// </summary>
[ExcludeFromCodeCoverage]
internal record StopPlaceOrStationRequestContract {
    /// <summary>
    /// Gets the board configurations
    /// </summary>
    [JsonPropertyName("boardConfigurations")]
    public List<string>? BoardConfigurations { get; init; }

    /// <summary>
    /// Gets the DHIDs
    /// </summary>
    [JsonPropertyName("dhids")]
    public List<string>? Dhids { get; init; }

    /// <summary>
    /// Gets the EVA numbers
    /// </summary>
    [JsonPropertyName("evaNumbers")]
    public List<string>? EvaNumbers { get; init; }

    /// <summary>
    /// Gets the platforms
    /// </summary>
    [JsonPropertyName("platforms")]
    public List<string>? Platforms { get; init; }

    /// <summary>
    /// Gets the STADA identifier
    /// </summary>
    [JsonPropertyName("stada")]
    public string? Stada { get; init; }
}
