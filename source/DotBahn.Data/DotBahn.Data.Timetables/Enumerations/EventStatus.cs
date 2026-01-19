using DotBahn.Data.Shared.Enumerations;

namespace DotBahn.Data.Timetables.Enumerations;

/// <summary>
/// Status of an arrival or departure event.
/// </summary>
public enum EventStatus {
    /// <summary>
    /// The event was planned. This status is also used when the cancellation of an event has been revoked.
    /// </summary>
    [AssociatedValue("p")]
    Planned,

    /// <summary>
    /// The event was added to the planned data (new stop).
    /// </summary>
    [AssociatedValue("a")]
    Added,

    /// <summary>
    /// The event was cancelled. As a changed status, this can apply to both planned and added stops.
    /// </summary>
    [AssociatedValue("c")]
    Cancelled,
    
    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
