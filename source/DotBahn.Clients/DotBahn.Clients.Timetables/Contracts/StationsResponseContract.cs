using System.Xml.Serialization;

namespace DotBahn.Timetables.Contracts;

/// <summary>
/// Root element for the station search response.
/// </summary>
[XmlRoot("stations")]
public record StationsResponseContract {
    /// <summary>
    /// List of stations found.
    /// </summary>
    [XmlElement("station")]
    public List<StationContract> Stations { get; init; } = [];
}
