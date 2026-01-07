using DotBahn.TimetableApi.Enumerations;
using DotBahn.TimetableApi.Models.Base;

namespace DotBahn.TimetableApi.Models;

/// <summary>
/// Represents information about a specific event, such as an arrival or a departure.
/// </summary>
public record EventInfo
{
    /// <summary>
    /// Gets the status of the event (e.g., Planned, Cancelled).
    /// </summary>
    public EventStatus Status { get; init; }

    /// <summary>
    /// Gets the planned and potentially changed time for the event.
    /// </summary>
    public ChangedValue<DateTime?> Time { get; init; } = new();

    /// <summary>
    /// Gets the planned and potentially changed platform for the event.
    /// </summary>
    public ChangedValue<string?> Platform { get; init; } = new();

    /// <summary>
    /// Gets the planned and potentially changed route or path for the event.
    /// </summary>
    public ChangedValue<List<string>?> Path { get; init; } = new();

    /// <summary>
    /// Gets the planned and potentially changed distant endpoint (e.g., destination for departure, origin for arrival).
    /// </summary>
    public ChangedValue<string?> DistantEndpoint { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether the event should be hidden from public displays.
    /// </summary>
    public bool IsHidden { get; init; }

    /// <summary>
    /// Gets the line information for the train (e.g., S1, RE5), if available.
    /// </summary>
    public string? Line { get; init; }

    /// <summary>
    /// Gets a value indicating whether the event has been cancelled.
    /// </summary>
    public bool IsCancelled => Status == EventStatus.Cancelled;
}