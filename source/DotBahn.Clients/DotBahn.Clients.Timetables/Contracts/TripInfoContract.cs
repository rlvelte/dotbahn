using System.Xml.Serialization;

namespace DotBahn.Clients.Timetables.Contracts;

/// <summary>
/// Raw attributes for trip label (train information)
/// </summary>
public record TripInfoContract {
    /// <summary>
    /// Filter flags
    /// </summary>
    [XmlAttribute("f")]
    public string? FilterFlags { get; init; }
    
    /// <summary>
    /// Trip type (p, e, f)
    /// </summary>
    [XmlAttribute("t")]
    public string? TripType { get; init; }
    
    /// <summary>
    /// Train owner
    /// </summary>
    [XmlAttribute("o")]
    public string? Owner { get; init; }
    
    /// <summary>
    /// Train category (e.g., ICE)
    /// </summary>
    [XmlAttribute("c")]
    public string? Category { get; init; }
    
    /// <summary>
    /// Train number
    /// </summary>
    [XmlAttribute("n")]
    public string? Number { get; init; }
}