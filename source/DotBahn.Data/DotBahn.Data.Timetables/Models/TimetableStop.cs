namespace DotBahn.Data.Timetables.Models;

/// <summary>
/// A stop in a timetable, representing a train's arrival and/or departure at a station.
/// </summary>
public class TimetableStop {
    /// <summary>
    /// Unique stop identifier consisting of:
    /// <list type="bullet">
    ///   <item><description>A daily trip ID that uniquely identifies a trip within one day (may be negative)</description></item>
    ///   <item><description>A 6-digit date specifier (YYMMdd) indicating the planned departure date from the start station</description></item>
    ///   <item><description>An index indicating the position of the stop within the trip</description></item>
    /// </list>
    /// </summary>
    /// <example>-7874571842864554321-140331-11</example>
    public required string Id { get; init; }

    /// <summary>
    /// Train identification information.
    /// </summary>
    public required TrainLabel Train { get; init; }

    /// <summary>
    /// Arrival event, if the train arrives at this station.
    /// </summary>
    public TrainEvent? Arrival { get; init; }
    
    /// <summary>
    /// Whether this is an arrival-only stop (train terminates here).
    /// </summary>
    public bool IsArrivalOnly => Arrival != null && Departure == null;

    /// <summary>
    /// Departure event, if the train departs from this station.
    /// </summary>
    public TrainEvent? Departure { get; init; }
    
    /// <summary>
    /// Whether this is a departure-only stop (train originates here).
    /// </summary>
    public bool IsDepartureOnly => Departure != null && Arrival == null;

    /// <summary>
    /// Whether this is a through stop (train both arrives and departs).
    /// </summary>
    public bool IsThrough => Arrival != null && Departure != null;
    
    /// <summary>
    /// Messages associated with this stop.
    /// </summary>
    public IEnumerable<TimetableMessage> Messages { get; init; } = [];
}
