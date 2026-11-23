using DotBahn.Boards.Models.Journeys;
using DotBahn.Boards.Models.Messages;
using DotBahn.Common.Models;

namespace DotBahn.Boards.Models.Entries;

/// <summary>
/// Shared data for an arrival or departure event at a stop place.
/// </summary>
public abstract class BoardEntry {
    /// <summary>
    /// Identifier of the event within its journey.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Stop place where the event occurs.
    /// </summary>
    public required StopPlace StopPlace { get; init; }

    /// <summary>
    /// Journey information.
    /// </summary>
    public required Journey Journey { get; init; }

    /// <summary>
    /// Scheduled and best-known event time.
    /// </summary>
    public required ChangedValue<DateTime> Time { get; init; }

    /// <summary>
    /// Scheduled and current platform, including accessibility information.
    /// </summary>
    public required ChangedRef<Platform> Platform { get; init; }

    /// <summary>
    /// Transport information including direction and via stops.
    /// </summary>
    public required Transport Transport { get; init; }

    /// <summary>
    /// Messages attached to this event.
    /// </summary>
    public IReadOnlyList<Message> Messages { get; init; } = [];

    /// <summary>
    /// Whether the event has been canceled.
    /// </summary>
    public bool Cancelled { get; init; }

    /// <summary>
    /// Whether the event is an additional stop.
    /// </summary>
    public bool Additional { get; init; }

    /// <summary>
    /// Whether the event is an on-demand stop.
    /// </summary>
    public bool OnDemand { get; init; }

    /// <summary>
    /// Whether passengers are not allowed to board or alight.
    /// </summary>
    public bool NoPassengerChange { get; init; }

    /// <summary>
    /// Whether a vehicle sequence is available for this event.
    /// </summary>
    public bool VehicleSequenceAvailable { get; init; }

    /// <summary>
    /// Airline codeshares for this event, keyed by IATA airline code with the flight number as value.
    /// </summary>
    public IReadOnlyDictionary<string, int> Codeshares { get; init; } = new Dictionary<string, int>();

    /// <summary>
    /// Journeys this event is relieved by.
    /// </summary>
    public IReadOnlyList<TransportReference> ReliefBy { get; init; } = [];

    /// <summary>
    /// Journeys this event reliefs.
    /// </summary>
    public IReadOnlyList<TransportReference> ReliefFor { get; init; } = [];

    /// <summary>
    /// Journeys this event is replaced by.
    /// </summary>
    public IReadOnlyList<TransportReference> ReplacedBy { get; init; } = [];

    /// <summary>
    /// Journeys this event replaces.
    /// </summary>
    public IReadOnlyList<TransportReference> ReplacementFor { get; init; } = [];

    /// <summary>
    /// Journeys this event travels with.
    /// </summary>
    public IReadOnlyList<TransportReference> TravelsWith { get; init; } = [];
}
