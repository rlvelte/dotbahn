using System.Xml.Serialization;

namespace DotBahn.Core.Contracts;

/// <summary>
/// Raw attributes for message/disruption information
/// </summary>
public record RawMessage {
    /// <summary>
    /// Message ID
    /// </summary>
    [XmlAttribute("id")]
    public string? Id { get; init; }
    
    /// <summary>
    /// Type
    /// </summary>
    [XmlAttribute("t")]
    public string? Type { get; init; }
    
    /// <summary>
    /// Valid from timestamp (YYMMDDhhmm)
    /// </summary>
    [XmlAttribute("from")]
    public string? ValidFrom { get; init; }
    
    /// <summary>
    /// Valid to timestamp (YYMMDDhhmm)
    /// </summary>
    [XmlAttribute("to")]
    public string? ValidTo { get; init; }
    
    /// <summary>
    /// Message code
    /// </summary>
    [XmlAttribute("c")]
    public string? Code { get; init; }
    
    /// <summary>
    /// Internal message flag
    /// </summary>
    [XmlAttribute("int")]
    public string? IsInternal { get; init; }
    
    /// <summary>
    /// Deleted flag
    /// </summary>
    [XmlAttribute("del")]
    public string? IsDeleted { get; init; }
    
    /// <summary>
    /// External category
    /// </summary>
    [XmlAttribute("ec")]
    public string? ExternalCategory { get; init; }
    
    /// <summary>
    /// Timestamp (YYMMDDhhmm)
    /// </summary>
    [XmlAttribute("ts")]
    public string? Timestamp { get; init; }
    
    /// <summary>
    /// Priority
    /// </summary>
    [XmlAttribute("priority")]
    public string? Priority { get; init; }
    
    /// <summary>
    /// Message owner
    /// </summary>
    [XmlAttribute("owner")]
    public string? Owner { get; init; }
    
    /// <summary>
    /// Category
    /// </summary>
    [XmlAttribute("cat")]
    public string? Category { get; init; }
    
    /// <summary>
    /// Message text content
    /// </summary>
    [XmlText]
    public string? Text { get; init; }
}