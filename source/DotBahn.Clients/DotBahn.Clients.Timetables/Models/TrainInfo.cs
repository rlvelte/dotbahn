using DotBahn.Timetables.Enumerations;

namespace DotBahn.Timetables.Models;

/// <summary>
/// Represents information about a specific train, including its category, number, and type.
/// </summary>
public record TrainInfo
{
    /// <summary>
    /// Gets the category of the train (e.g., ICE, RE, S).
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Gets the train number.
    /// </summary>
    public string Number { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of the trip (e.g., National, Regional).
    /// </summary>
    public TripType Type { get; init; }

    /// <summary>
    /// Gets the owner or operator of the train, if available.
    /// </summary>
    public string? Owner { get; init; }

    /// <summary>
    /// Gets flags used for filtering or classification, if available.
    /// </summary>
    public string? FilterFlags { get; init; }
}