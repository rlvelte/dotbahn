using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a line information
/// </summary>
[ExcludeFromCodeCoverage]
internal record LineInfoContract {
    /// <summary>
    /// Gets the colour
    /// </summary>
    [JsonPropertyName("colour")]
    public LineColourContract? Colour { get; init; }

    /// <summary>
    /// Gets the name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
