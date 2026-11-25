using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a line colour.
/// </summary>
[ExcludeFromCodeCoverage]
internal record LineColourContract {
    /// <summary>
    /// Gets the chip visualization.
    /// </summary>
    [JsonPropertyName("chipVisualization")]
    public LineChipVisualizationContract? ChipVisualization { get; init; }

    /// <summary>
    /// Gets the primary colour.
    /// </summary>
    [JsonPropertyName("primaryColour")]
    public string PrimaryColour { get; init; } = string.Empty;

    /// <summary>
    /// Gets the primary text colour.
    /// </summary>
    [JsonPropertyName("primaryTextColour")]
    public string PrimaryTextColour { get; init; } = string.Empty;

    /// <summary>
    /// Gets the stroke visualization.
    /// </summary>
    [JsonPropertyName("strokeVisualization")]
    public LineStrokeVisualizationContract? StrokeVisualization { get; init; }
}
