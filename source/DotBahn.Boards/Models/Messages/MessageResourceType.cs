namespace DotBahn.Boards.Models.Messages;

/// <summary>
/// Type of resource attached to a message.
/// </summary>
public enum MessageResourceType {
    /// <summary>
    /// A hyperlink.
    /// </summary>
    Link,

    /// <summary>
    /// An attachment.
    /// </summary>
    Attachment,

    /// <summary>
    /// An image.
    /// </summary>
    Image
}
