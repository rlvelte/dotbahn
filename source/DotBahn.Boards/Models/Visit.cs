namespace DotBahn.Boards.Models;

/// <summary>
/// A reference to a stop place with visit-specific details.
/// </summary>
public class Visit {
    /// <summary>
    /// The referenced stop place.
    /// </summary>
    public required StopPlace StopPlace { get; init; }

    /// <summary>
    /// Whether this is an additional stop.
    /// </summary>
    public bool Additional { get; init; }

    /// <summary>
    /// Whether this stop has been canceled.
    /// </summary>
    public bool Cancelled { get; init; }

    /// <summary>
    /// Display priority in ascending order.
    /// </summary>
    public int? DisplayPriority { get; init; }
}
