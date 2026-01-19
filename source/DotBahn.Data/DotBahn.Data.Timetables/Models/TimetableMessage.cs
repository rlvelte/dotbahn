using DotBahn.Data.Timetables.Enumerations;

namespace DotBahn.Data.Timetables.Models;

/// <summary>
/// A message associated with an event, a stop, or a trip.
/// Messages can represent disruptions, delays, quality changes, or general information.
/// </summary>
public class TimetableMessage {
    /// <summary>
    /// Unique message identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Type of message indicating its source and purpose.
    /// </summary>
    public required MessageType Type { get; init; }

    /// <summary>
    /// Timestamp when the message was created.
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    /// Message code.
    /// </summary>
    public int? Code { get; init; }

    /// <summary>
    /// Message category.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// External category for the message.
    /// </summary>
    public string? ExternalCategory { get; init; }

    /// <summary>
    /// Message priority level.
    /// </summary>
    public MessagePriority? Priority { get; init; }

    /// <summary>
    /// Message owner/source.
    /// </summary>
    public string? Owner { get; init; }

    /// <summary>
    /// Start of the message validity period.
    /// </summary>
    public DateTime? ValidFrom { get; init; }

    /// <summary>
    /// End of the message validity period.
    /// </summary>
    public DateTime? ValidTo { get; init; }

    /// <summary>
    /// Internal text (not intended for public display).
    /// </summary>
    public string? InternalText { get; init; }

    /// <summary>
    /// External text (public-facing message content).
    /// </summary>
    public string? ExternalText { get; init; }

    /// <summary>
    /// External link associated with the message.
    /// </summary>
    public string? ExternalLink { get; init; }

    /// <summary>
    /// Whether the message has been deleted.
    /// </summary>
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Trip labels of affected trains.
    /// </summary>
    public IReadOnlyList<TrainLabel> AffectedTrips { get; init; } = [];

    /// <summary>
    /// The display text, preferring external text over internal text.
    /// </summary>
    public string? Text => ExternalText ?? InternalText;

    /// <summary>
    /// Whether the message is currently active (not deleted and within validity period).
    /// </summary>
    public bool IsActive => !IsDeleted && (ValidTo == null || ValidTo > DateTime.Now);
}
