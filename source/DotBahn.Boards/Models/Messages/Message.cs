namespace DotBahn.Boards.Models.Messages;

/// <summary>
/// A message attached to a board entry or station.
/// </summary>
public class Message {
    /// <summary>
    /// Message text.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// Type of the message.
    /// </summary>
    public required MessageType Type { get; init; }

    /// <summary>
    /// Optional short message text.
    /// </summary>
    public string? ShortText { get; init; }

    /// <summary>
    /// Optional category.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Optional code.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Display priority in ascending order.
    /// </summary>
    public int? Priority { get; init; }

    /// <summary>
    /// Detailed display priority in ascending order.
    /// </summary>
    public int? DetailedPriority { get; init; }

    /// <summary>
    /// Cause for disruption messages.
    /// </summary>
    public string? Cause { get; init; }

    /// <summary>
    /// Effect of disruption messages.
    /// </summary>
    public string? Effect { get; init; }

    /// <summary>
    /// Identifier of the associated disruption, if available.
    /// </summary>
    public string? DisruptionId { get; init; }

    /// <summary>
    /// Identifier of the associated disruption communication, if available.
    /// </summary>
    public string? DisruptionCommunicationId { get; init; }

    /// <summary>
    /// Whether alternative connections exist for this message.
    /// </summary>
    public bool HasAlternatives { get; init; }

    /// <summary>
    /// Related resources such as links, attachments, and images.
    /// </summary>
    public IReadOnlyList<MessageResource> Resources { get; init; } = [];
}
