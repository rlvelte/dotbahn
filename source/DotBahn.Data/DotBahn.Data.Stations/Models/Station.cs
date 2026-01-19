using DotBahn.Data.Shared.Models;
using DotBahn.Data.Stations.Enumerations;

namespace DotBahn.Data.Stations.Models;

/// <summary>
/// A railway station with its identification, location, and available services.
/// </summary>
public class Station {
    /// <summary>
    /// Unique station number assigned by DB Station & Service.
    /// </summary>
    public required int Number { get; init; }

    /// <summary>
    /// Official name of the station.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Station category indicating its importance and size.
    /// </summary>
    public required StationCategory Category { get; init; }

    /// <summary>
    /// Mailing address of the station.
    /// </summary>
    public StationAddress? Address { get; init; }

    /// <summary>
    /// Regional area of Deutsche Bahn responsible for the station.
    /// </summary>
    public RegionalArea? RegionalArea { get; init; }

    /// <summary>
    /// RIL 100 identifiers for the station.
    /// </summary>
    public IEnumerable<Ril100Identifier> Ril100Identifiers { get; init; } = [];

    /// <summary>
    /// EVA numbers for the station.
    /// </summary>
    public IEnumerable<EvaNumber> EvaNumbers { get; init; } = [];

    /// <summary>
    /// Available services and facilities at the station.
    /// </summary>
    public required StationServices Services { get; init; }

    /// <summary>
    /// Primary RIL 100 identifier for the station.
    /// </summary>
    public Ril100Identifier? PrimaryRil100 => 
        Ril100Identifiers.FirstOrDefault(r => r.IsMain) ?? Ril100Identifiers.FirstOrDefault();

    /// <summary>
    /// Primary EVA number for the station.
    /// </summary>
    public EvaNumber? PrimaryEva =>
        EvaNumbers.FirstOrDefault(e => e.IsMain) ?? EvaNumbers.FirstOrDefault();

    /// <summary>
    /// Primary geographic coordinates of the station.
    /// </summary>
    public Coordinates? Coordinates => 
        PrimaryEva?.Coordinates;
}
