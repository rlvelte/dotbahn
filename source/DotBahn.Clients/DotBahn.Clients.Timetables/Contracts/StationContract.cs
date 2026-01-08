using System.Xml.Serialization;

namespace DotBahn.Timetables.Contracts;

/// <summary>
/// Raw XML structure for a single station from the station search endpoint.
/// </summary>
public record StationContract {
    /// <summary>
    /// The name of the station.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The EVA station number.
    /// </summary>
    [XmlAttribute("eva")]
    public string Eva { get; init; } = string.Empty;
}
