using DotBahn.Timetables.Enumerations;

namespace DotBahn.Timetables.Models;

/// <summary>
/// Represents a message or disruption information related to a train or station.
/// </summary>
public record Message {
    /// <summary>
    /// Gets the unique identifier for the message.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of the message (e.g., Quality, Disruption).
    /// </summary>
    public MessageType Type { get; init; }

    /// <summary>
    /// Gets the numeric or alphanumeric code for the message, if available.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the descriptive text of the message, if available.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// Gets the date and time from which the message is valid, if applicable.
    /// </summary>
    public DateTime? ValidFrom { get; init; }

    /// <summary>
    /// Gets the date and time until which the message is valid, if applicable.
    /// </summary>
    public DateTime? ValidTo { get; init; }

    /// <summary>
    /// Gets the timestamp when the message was created or last updated, if available.
    /// </summary>
    public DateTime? Timestamp { get; init; }

    /// <summary>
    /// Gets the priority level of the message, if available.
    /// </summary>
    public int? Priority { get; init; }

    /// <summary>
    /// Gets a value indicating whether the message is intended for internal use only.
    /// </summary>
    public bool IsInternal { get; init; }

    /// <summary>
    /// Gets a value indicating whether the message has been deleted.
    /// </summary>
    public bool IsDeleted { get; init; }
}