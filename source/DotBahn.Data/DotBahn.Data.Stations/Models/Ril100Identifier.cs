namespace DotBahn.Data.Stations.Models;

/// <summary>
/// A RIL 100 identifier (Richtlinie 100) used to uniquely identify railway operating points.
/// </summary>
public class Ril100Identifier {
    /// <summary>
    /// The RIL 100 abbreviation code.
    /// </summary>
    public required string Identifier { get; init; }

    /// <summary>
    /// Whether this is the primary RIL 100 identifier for the station.
    /// A station may have multiple identifiers for different operational areas.
    /// </summary>
    public required bool IsMain { get; init; }
}
