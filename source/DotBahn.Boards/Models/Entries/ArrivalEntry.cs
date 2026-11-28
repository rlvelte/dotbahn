using DotBahn.Boards.Models.Journeys;

namespace DotBahn.Boards.Models.Entries;

/// <summary>
/// An arrival event at a stop place.
/// </summary>
public sealed class ArrivalEntry : BoardEntry {
    /// <summary>
    /// Journeys this arrival continues as.
    /// </summary>
    public IReadOnlyList<TransportReference> ContinuationBy { get; init; } = [];

    /// <summary>
    /// Whether at least one disruption communication exists in the past of this journey.
    /// </summary>
    public bool PastDisruptions { get; init; }
}
