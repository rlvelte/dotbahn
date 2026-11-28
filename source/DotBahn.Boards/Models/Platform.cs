namespace DotBahn.Boards.Models;

/// <summary>
/// Platform information for a board entry.
/// </summary>
public class Platform {
    /// <summary>
    /// Name of the platform.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// IFOPT / DHID of the platform.
    /// </summary>
    public string? Ifopt { get; init; }

    /// <summary>
    /// Whether step-free access is available.
    /// </summary>
    public bool StepFreeAccess { get; init; }

    /// <summary>
    /// Whether a boarding aid is available.
    /// </summary>
    public bool BoardingAid { get; init; }

    /// <summary>
    /// Whether tactile guiding strips are available.
    /// </summary>
    public bool TactileGuidingStrips { get; init; }

    /// <summary>
    /// Whether tactile handrail labels are available.
    /// </summary>
    public bool TactileHandrailLabel { get; init; }

    /// <summary>
    /// Whether tactile platform access indicators are available.
    /// </summary>
    public bool TactilePlatformAccess { get; init; }

    /// <summary>
    /// Whether audible signals are available.
    /// </summary>
    public bool AudibleSignalsAvailable { get; init; }

    /// <summary>
    /// Whether automatic doors are available.
    /// </summary>
    public bool AutomaticDoor { get; init; }

    /// <summary>
    /// Whether a passenger information display is available.
    /// </summary>
    public bool PassengerInformationDisplay { get; init; }

    /// <summary>
    /// Whether a platform sign is available.
    /// </summary>
    public bool PlatformSign { get; init; }

    /// <summary>
    /// Whether stairs marking is available.
    /// </summary>
    public bool StairsMarking { get; init; }

    /// <summary>
    /// Whether the platform has a standard height.
    /// </summary>
    public bool StandardPlatformHeight { get; init; }
}
