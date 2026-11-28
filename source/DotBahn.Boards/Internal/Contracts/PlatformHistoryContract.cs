using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a platform history entry.
/// </summary>
[ExcludeFromCodeCoverage]
internal record PlatformHistoryContract {
    /// <summary>
    /// Gets the IFOPT.
    /// </summary>
    [JsonPropertyName("ifopt")]
    public string? Ifopt { get; init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
