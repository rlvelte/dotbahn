using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Data.Timetables.Enumerations;

/// <summary>
/// Type of timetable message.
/// </summary>
public enum MessageType {
    /// <summary>
    /// A HIM message (generated through the Hafas Information Manager).
    /// </summary>
    [AssociatedValue("h")]
    Him,

    /// <summary>
    /// A message about a quality change.
    /// </summary>
    [AssociatedValue("q")]
    QualityChange,

    /// <summary>
    /// A free text message.
    /// </summary>
    [AssociatedValue("f")]
    Free,

    /// <summary>
    /// A message about the cause of a delay.
    /// </summary>
    [AssociatedValue("d")]
    CauseOfDelay,

    /// <summary>
    /// An IBIS message (generated from IRIS-AP).
    /// </summary>
    [AssociatedValue("i")]
    Ibis,

    /// <summary>
    /// An IBIS message (generated from IRIS-AP) not yet assigned to a train.
    /// </summary>
    [AssociatedValue("u")]
    UnassignedIbis,

    /// <summary>
    /// A major disruption.
    /// </summary>
    [AssociatedValue("r")]
    Disruption,

    /// <summary>
    /// A connection message.
    /// </summary>
    [AssociatedValue("c")]
    Connection,
    
    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
