using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Clients.Timetables.Enumerations;

/// <summary>
/// Defines the possible statuses of a train event (arrival or departure).
/// </summary>
public enum EventStatus {
    /// <summary>
    /// The event is planned as scheduled.
    /// </summary>
    [AssociatedValue("p")]
    Planned,

    /// <summary>
    /// The event has been added as an extra or unscheduled stop.
    /// </summary>
    [AssociatedValue("a")]
    Added,  

    /// <summary>
    /// The event has been canceled.
    /// </summary>
    [AssociatedValue("c")]
    Cancelled
}