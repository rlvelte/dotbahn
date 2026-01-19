namespace DotBahn.Data.Stations.Models;

/// <summary>
/// An EVA (Europäische Verkehrsanlagen) number uniquely identifying a railway station or stop.
/// </summary>
public class EvaNumber {
    /// <summary>
    /// The EVA number.
    /// </summary>
    public required long Number { get; init; }

    /// <summary>
    /// Whether this is the primary EVA number for the station.
    /// Large stations may have multiple EVA numbers for different platform areas.
    /// </summary>
    public required bool IsMain { get; init; }

    /// <summary>
    /// Geographic coordinates associated with this EVA number.
    /// </summary>
    public Coordinates? Coordinates { get; init; }
}
