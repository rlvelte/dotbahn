using System.Xml.Serialization;

namespace DotBahn.Timetable.Contracts;

/// <summary>
/// Raw attributes for trip label (train information)
/// </summary>
public record TripLabelContract {
    [XmlAttribute("f")]
    public string? FilterFlags { get; init; }
    
    [XmlAttribute("t")]
    public string? TripType { get; init; }
    
    [XmlAttribute("o")]
    public string? Owner { get; init; }
    
    [XmlAttribute("c")]
    public string? Category { get; init; }
    
    [XmlAttribute("n")]
    public string? Number { get; init; }
}