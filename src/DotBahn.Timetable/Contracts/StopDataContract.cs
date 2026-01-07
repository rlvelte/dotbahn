using System.Xml.Serialization;
using DotBahn.Core.Contracts;

namespace DotBahn.Timetable.Contracts;

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
    
    [XmlElement("tl")]
    public TripLabelContract? TripLabel { get; init; }
    
    [XmlElement("ar")]
    public EventContract? Arrival { get; init; }
    
    [XmlElement("dp")]
    public EventContract? Departure { get; init; }
    
    [XmlElement("m")]
    public List<RawMessage>? Messages { get; init; }
}