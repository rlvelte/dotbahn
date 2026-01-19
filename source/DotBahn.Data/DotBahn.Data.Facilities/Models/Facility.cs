using DotBahn.Data.Facilities.Enumerations;
using DotBahn.Data.Shared.Models;

namespace DotBahn.Data.Facilities.Models;

/// <summary>
/// A station facility such as an elevator or escalator with operational status information.
/// </summary>
public class Facility {
    /// <summary>
    /// Unique equipment number assigned by Deutsche Bahn.
    /// </summary>
    public required long EquipmentNumber { get; init; }

    /// <summary>
    /// Type of facility (elevator or escalator).
    /// </summary>
    public required FacilityType Type { get; init; }

    /// <summary>
    /// Human-readable description of the facility location and purpose.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Current operational state of the facility.
    /// </summary>
    public required FacilityState State { get; init; }

    /// <summary>
    /// Explanation for the current state, typically provided when the facility is inactive.
    /// </summary>
    public string? StateExplanation { get; init; }

    /// <summary>
    /// Station number this facility belongs to.
    /// </summary>
    public required int StationNumber { get; init; }

    /// <summary>
    /// Geographic coordinates of the facility within the station.
    /// </summary>
    public required Coordinates Coordinates { get; init; }

    /// <summary>
    /// Name of the operator responsible for maintaining this facility.
    /// </summary>
    public string? OperatorName { get; init; }
}
