namespace DotBahn.Boards.Models.Messages;

/// <summary>
/// A resource attached to a message, such as a link, attachment, or image.
/// </summary>
public class MessageResource {
    /// <summary>
    /// Type of the resource.
    /// </summary>
    public required MessageResourceType Type { get; init; }

    /// <summary>
    /// Label for the resource.
    /// </summary>
    public required string Label { get; init; }

    /// <summary>
    /// URL of the resource.
    /// </summary>
    public required string Url { get; init; }
}
