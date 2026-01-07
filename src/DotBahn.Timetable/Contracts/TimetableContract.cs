using System.Xml.Serialization;

namespace DotBahn.Timetable.Contracts;

/// <summary>
/// Raw XML timetable response
/// </summary>
[XmlRoot("timetable")]
public record TimetableContract {
    [XmlAttribute("station")]
    public string Station { get; init; } = string.Empty;

    [XmlElement("s")]
    public List<StopDataContract> Stops { get; init; } = [];
}