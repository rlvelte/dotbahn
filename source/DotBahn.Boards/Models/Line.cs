namespace DotBahn.Boards.Models;

/// <summary>
/// Line information for a transport, including color and visualization details.
/// </summary>
public class Line {
    /// <summary>
    /// Name of the line, e.g. RE14 or S1.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Primary color as a HEX code with leading #.
    /// </summary>
    public string? PrimaryColour { get; init; }

    /// <summary>
    /// Primary text color as a HEX code with leading #.
    /// </summary>
    public string? PrimaryTextColour { get; init; }

    /// <summary>
    /// Differing boundary color of the chip visualization.
    /// </summary>
    public string? ChipDifferingBoundaryColour { get; init; }

    /// <summary>
    /// Differing fill color of the chip visualization.
    /// </summary>
    public string? ChipDifferingFillColour { get; init; }

    /// <summary>
    /// Differing text color of the chip visualization.
    /// </summary>
    public string? ChipDifferingTextColour { get; init; }

    /// <summary>
    /// Secondary fill color of the chip visualization.
    /// </summary>
    public string? ChipSecondaryFillColour { get; init; }

    /// <summary>
    /// Differing boundary color of the stroke visualization.
    /// </summary>
    public string? StrokeDifferingBoundaryColour { get; init; }

    /// <summary>
    /// Secondary fill color of the stroke visualization.
    /// </summary>
    public string? StrokeSecondaryFillColour { get; init; }
}
