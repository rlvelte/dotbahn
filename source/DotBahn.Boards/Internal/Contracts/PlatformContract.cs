using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a platform
/// </summary>
[ExcludeFromCodeCoverage]
internal record PlatformContract {
    /// <summary>
    /// Gets the accessibility
    /// </summary>
    [JsonPropertyName("accessibility")]
    public PlatformAccessibilityContract? Accessibility { get; init; }

    /// <summary>
    /// Gets the IFOPT
    /// </summary>
    [JsonPropertyName("ifopt")]
    public string? Ifopt { get; init; }

    /// <summary>
    /// Gets the name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
