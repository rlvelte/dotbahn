using DotBahn.Boards.Models.Operators;

namespace DotBahn.Boards.Models.Journeys;

/// <summary>
/// Journey-level information for a board entry.
/// </summary>
public class Journey {
    /// <summary>
    /// Identifier of the journey.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Origin stop place, including cancellation state.
    /// </summary>
    public required Visit Origin { get; init; }

    /// <summary>
    /// Destination stop place, including cancellation state.
    /// </summary>
    public required Visit Destination { get; init; }

    /// <summary>
    /// Differing origin stop place, if the journey starts elsewhere.
    /// </summary>
    public StopPlace? DifferingOrigin { get; init; }

    /// <summary>
    /// Differing destination stop place, if the journey ends elsewhere.
    /// </summary>
    public StopPlace? DifferingDestination { get; init; }

    /// <summary>
    /// Scheduled start time at the origin.
    /// </summary>
    public required DateTime ScheduledStartTime { get; init; }

    /// <summary>
    /// Transport information at the start of the journey.
    /// </summary>
    public required Transport TransportAtStart { get; init; }

    /// <summary>
    /// Whether the entire journey has been canceled.
    /// </summary>
    public bool IsCancelled { get; init; }

    /// <summary>
    /// Journey type, e.g. REGULAR, REPLACEMENT, RELIEF, EXTRA.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Administration responsible for the journey header, if available.
    /// </summary>
    public Administration? HeaderAdministration { get; init; }

    /// <summary>
    /// Journey number of the header, if available.
    /// </summary>
    public int? HeaderJourneyNumber { get; init; }
}
