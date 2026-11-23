using DotBahn.Boards.Models.Journeys;

namespace DotBahn.Boards.Models.Entries;

/// <summary>
/// A departure event at a stop place.
/// </summary>
public sealed class DepartureEntry : BoardEntry {
    /// <summary>
    /// Journeys this departure continues for.
    /// </summary>
    public IReadOnlyList<TransportReference> ContinuationFor { get; init; } = [];

    /// <summary>
    /// Whether at least one disruption communication exists in the future of this journey.
    /// </summary>
    public bool FutureDisruptions { get; init; }
}
