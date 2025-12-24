using DotDB.Models.Enumerations;

namespace DotDB.Models;

/// <summary>
/// Managed message/disruption information
/// </summary>
public class Message
{
    public string Id { get; set; }            // Message ID
    public MessageType Type { get; set; }     // Message type
    public string Code { get; set; }          // Message code
    public string Text { get; set; }          // Message text
    public DateTime? ValidFrom { get; set; }  // Valid from
    public DateTime? ValidTo { get; set; }    // Valid to
    public DateTime? Timestamp { get; set; }  // Timestamp
    public int? Priority { get; set; }        // Priority level
    public bool IsInternal { get; set; }      // Internal message flag
    public bool IsDeleted { get; set; }       // Deleted flag
}