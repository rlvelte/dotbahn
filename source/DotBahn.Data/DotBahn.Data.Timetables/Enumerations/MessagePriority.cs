namespace DotBahn.Data.Timetables.Enumerations;

/// <summary>
/// Priority level of a timetable message.
/// </summary>
public enum MessagePriority {
    /// <summary>
    /// High priority message.
    /// </summary>
    High = 1,

    /// <summary>
    /// Medium priority message.
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Low priority message.
    /// </summary>
    Low = 3,

    /// <summary>
    /// Message has been resolved/done.
    /// </summary>
    Done = 4,
    
    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
