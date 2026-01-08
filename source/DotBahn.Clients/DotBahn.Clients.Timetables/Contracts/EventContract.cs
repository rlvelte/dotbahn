using System.Xml.Serialization;

namespace DotBahn.Clients.Timetables.Contracts;

/// <summary>
/// Raw event information (Arrival/Departure)
/// </summary>
public record EventContract {
    /// <summary>
    /// Planned time (YYMMDDhhmm format)
    /// </summary>
    [XmlAttribute("pt")]
    public string? PlannedTime { get; init; }
    
    /// <summary>
    /// Planned platform
    /// </summary>
    [XmlAttribute("pp")]
    public string? PlannedPlatform { get; init; }
    
    /// <summary>
    /// Planned status (e.g., "p" for planned)
    /// </summary>
    [XmlAttribute("ps")]
    public string? PlannedStatus { get; init; }
    
    /// <summary>
    /// Changed time (YYMMDDhhmm format)
    /// </summary>
    [XmlAttribute("ct")]
    public string? ChangedTime { get; init; }
    
    /// <summary>
    /// Changed platform
    /// </summary>
    [XmlAttribute("cp")]
    public string? ChangedPlatform { get; init; } 
    
    /// <summary>
    /// Changed status (e.g., "c" for canceled)
    /// </summary>
    [XmlAttribute("cs")]
    public string? ChangedStatus { get; init; }    
    
    /// <summary>
    /// Hidden (0 or 1)
    /// </summary>
    [XmlAttribute("hi")]
    public string? IsHidden { get; init; }
    
    /// <summary>
    /// Line
    /// </summary>
    [XmlAttribute("l")]
    public string? Line { get; init; }
    
    /// <summary>
    /// Planned path (pipe-separated station list)
    /// </summary>
    [XmlAttribute("ppth")]
    public string? PlannedPath { get; init; }
    
    /// <summary>
    /// Changed path (pipe-separated station list)
    /// </summary>
    [XmlAttribute("cpth")]
    public string? ChangedPath { get; init; }
    
    /// <summary>
    /// Wing train information
    /// </summary>
    [XmlAttribute("wings")]
    public string? Wings { get; init; }
    
    /// <summary>
    /// Transition information
    /// </summary>
    [XmlAttribute("tra")]
    public string? Transition { get; init; }
    
    /// <summary>
    /// Planned distant endpoint
    /// </summary>
    [XmlAttribute("pde")]
    public string? PlannedDistantEndpoint { get; init; }
    
    /// <summary>
    /// Changed distant endpoint
    /// </summary>
    [XmlAttribute("cde")]
    public string? ChangedDistantEndpoint { get; init; }
}
