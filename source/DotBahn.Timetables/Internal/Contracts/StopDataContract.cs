using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Timetables.Internal.Contracts;

/// <summary>
/// Raw structure for a single stop
/// </summary>
[ExcludeFromCodeCoverage]
internal record StopDataContract {
    /// <summary>
    /// Unique stop ID
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// EVA number of the station
    /// </summary>
    public string Eva { get; init; } = string.Empty;

    /// <summary>
    /// Trip label information
    /// </summary>
    public TripInfoContract? TripInfo { get; init; }

    /// <summary>
    /// Arrival information
    /// </summary>
    public EventContract? Arrival { get; init; }

    /// <summary>
    /// Departure information
    /// </summary>
    public EventContract? Departure { get; init; }

    /// <summary>
    /// List of messages
    /// </summary>
    public List<MessageContract>? Messages { get; init; }
}
