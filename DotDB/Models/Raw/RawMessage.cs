namespace DotDB.Models.Raw;

/// <summary>
/// Raw attributes for message/disruption information
/// </summary>
public record RawMessage {
    /// <summary>
    /// Message ID
    /// </summary>
    public string Id { get; init; }
    
    /// <summary>
    /// Type
    /// </summary>
    public string T { get; init; }
    
    /// <summary>
    /// Valid from timestamp (YYMMDDhhmm)
    /// </summary>
    public string From { get; init; }
    
    /// <summary>
    /// Valid to timestamp (YYMMDDhhmm)
    /// </summary>
    public string To { get; init; }
    
    /// <summary>
    /// Message code
    /// </summary>
    public string C { get; init; }
    
    /// <summary>
    /// Internal message flag
    /// </summary>
    public string Int { get; init; }
    
    /// <summary>
    /// Deleted flag
    /// </summary>
    public string Del { get; init; }
    
    /// <summary>
    /// External category
    /// </summary>
    public string Ec { get; set; }
    
    /// <summary>
    /// Timestamp (YYMMDDhhmm)
    /// </summary>
    public string Ts { get; init; }
    
    /// <summary>
    /// Priority
    /// </summary>
    public string Priority { get; init; }
    
    /// <summary>
    /// Message owner
    /// </summary>
    public string Owner { get; set; }
    
    /// <summary>
    /// Category
    /// </summary>
    public string Cat { get; set; }
    
    /// <summary>
    /// Message text content
    /// </summary>
    public string Text { get; init; }
}