using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a line chip visualization.
/// </summary>
[ExcludeFromCodeCoverage]
internal record LineChipVisualizationContract {
    /// <summary>
    /// Gets the differing boundary colour.
    /// </summary>
    [JsonPropertyName("differingBoundaryColour")]
    public string? DifferingBoundaryColour { get; init; }

    /// <summary>
    /// Gets the differing fill colour.
    /// </summary>
    [JsonPropertyName("differingFillColour")]
    public string? DifferingFillColour { get; init; }

    /// <summary>
    /// Gets the differing text colour.
    /// </summary>
    [JsonPropertyName("differingTextColour")]
    public string? DifferingTextColour { get; init; }

    /// <summary>
    /// Gets the secondary fill colour.
    /// </summary>
    [JsonPropertyName("secondaryFillColour")]
    public string? SecondaryFillColour { get; init; }
}
