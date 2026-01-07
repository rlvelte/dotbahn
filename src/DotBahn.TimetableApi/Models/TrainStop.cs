namespace DotBahn.TimetableApi.Models;

/// <summary>
/// Represents a train stop at a specific station, including arrival and departure details.
/// </summary>
public record TrainStop {
    /// <summary>
    /// Gets the unique identifier for this train stop.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the information about the station where the stop occurs.
    /// </summary>
    public StationInfo Station { get; init; } = new();

    /// <summary>
    /// Gets the information about the train associated with this stop.
    /// </summary>
    public TrainInfo Train { get; init; } = new();

    /// <summary>
    /// Gets the arrival event details, if applicable.
    /// </summary>
    public EventInfo? Arrival { get; init; }

    /// <summary>
    /// Gets the departure event details, if applicable.
    /// </summary>
    public EventInfo? Departure { get; init; }

    /// <summary>
    /// Gets the list of messages or notifications associated with this train stop.
    /// </summary>
    public List<Message> Messages { get; init; } = [];
}