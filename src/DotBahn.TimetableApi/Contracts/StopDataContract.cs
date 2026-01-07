using System.Xml.Serialization;
using DotBahn.Core.Contracts;

namespace DotBahn.TimetableApi.Contracts;

/// <summary>
/// Raw XML structure for a single stop
/// </summary>
public record StopDataContract {
    /// <summary>
    /// Unique stop ID
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// EVA number of the station
    /// </summary>
    [XmlAttribute("eva")]
    public string Eva { get; init; } = string.Empty;
    
    /// <summary>
    /// Trip label information
    /// </summary>
    [XmlElement("tl")]
    public TripLabelContract? TripLabel { get; init; }
    
    /// <summary>
    /// Arrival information
    /// </summary>
    [XmlElement("ar")]
    public EventContract? Arrival { get; init; }
    
    /// <summary>
    /// Departure information
    /// </summary>
    [XmlElement("dp")]
    public EventContract? Departure { get; init; }
    
    /// <summary>
    /// List of messages
    /// </summary>
    [XmlElement("m")]
    public List<RawMessage>? Messages { get; init; }
}