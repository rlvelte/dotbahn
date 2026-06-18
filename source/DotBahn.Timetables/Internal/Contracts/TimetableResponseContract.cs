using System.Xml.Serialization;

namespace DotBahn.Timetables.Internal.Contracts;

/// <summary>
/// Raw XML timetable response
/// </summary>
[XmlRoot("timetable")]
internal record TimetableResponseContract {
    /// <summary>
    /// Station name or ID
    /// </summary>
    [XmlAttribute("station")]
    public string Station { get; init; } = string.Empty;

    /// <summary>
    /// List of stops in the timetable
    /// </summary>
    [XmlElement("s")]
    public List<StopDataContract> Stops { get; init; } = [];
}
