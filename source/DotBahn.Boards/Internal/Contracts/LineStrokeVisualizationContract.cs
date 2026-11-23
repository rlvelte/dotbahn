using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a line stroke visualization
/// </summary>
[ExcludeFromCodeCoverage]
internal record LineStrokeVisualizationContract {
    /// <summary>
    /// Gets the differing boundary colour
    /// </summary>
    [JsonPropertyName("differingBoundaryColour")]
    public string? DifferingBoundaryColour { get; init; }

    /// <summary>
    /// Gets the secondary fill colour
    /// </summary>
    [JsonPropertyName("secondaryFillColour")]
    public string? SecondaryFillColour { get; init; }
}
