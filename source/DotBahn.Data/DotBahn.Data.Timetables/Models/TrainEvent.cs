using DotBahn.Data.Shared.Models;
using DotBahn.Data.Timetables.Enumerations;

namespace DotBahn.Data.Timetables.Models;

/// <summary>
/// Represents an arrival or departure event at a station.
/// </summary>
public class TrainEvent {
    /// <summary>
    /// The time of this event
    /// </summary>
    public required ChangedValue<DateTime> Time { get; init; }
    
    /// <summary>
    /// The platform this event occurs on.
    /// </summary>
    public required ChangedRef<string> Platform { get; init; }

    /// <summary>
    /// The event status.
    /// </summary>
    public required ChangedValue<EventStatus> Status { get; init; }
    
    /// <summary>
    /// Planned distant endpoint.
    /// For international trains, this indicates the final destination beyond the stations listed in the path.
    /// </summary>
    public required ChangedRef<string> DistantEndpoint { get; init; }

    /// <summary>
    /// Planned path as a sequence of station names.
    /// For arrival, the path indicates stations before the current station (the first element is trip's start).
    /// For departure, the path indicates stations after the current station (the last element is the trip's destination).
    /// The current station is never included in the path.
    /// </summary>
    public required ChangedRef<IEnumerable<string>> Path { get; init; }
    
    /// <summary>
    /// Wing train IDs. A list of trip IDs for coupled trains (train portions).
    /// </summary>
    public IEnumerable<string> Wings { get; init; } = [];
    
    /// <summary>
    /// Messages associated with this event.
    /// </summary>
    public IEnumerable<TimetableMessage> Messages { get; init; } = [];
}
