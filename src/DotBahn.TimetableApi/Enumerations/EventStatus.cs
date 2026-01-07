namespace DotBahn.TimetableApi.Enumerations;

/// <summary>
/// Defines the possible statuses of a train event (arrival or departure).
/// </summary>
public enum EventStatus {
    /// <summary>
    /// The event is planned as scheduled.
    /// </summary>
    Planned,

    /// <summary>
    /// The event has been added as an extra or unscheduled stop.
    /// </summary>
    Added,  

    /// <summary>
    /// The event has been canceled.
    /// </summary>
    Cancelled
}