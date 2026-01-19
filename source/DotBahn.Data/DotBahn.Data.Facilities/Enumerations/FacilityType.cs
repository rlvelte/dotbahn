using DotBahn.Data.Shared.Enumerations;

namespace DotBahn.Data.Facilities.Enumerations;

/// <summary>
/// Represents the type of facility in a station.
/// </summary>
public enum FacilityType {
    /// <summary>
    /// Elevator facility.
    /// </summary>
    [AssociatedValue("ELEVATOR")]
    Elevator,
    
    /// <summary>
    /// Escalator facility.
    /// </summary>
    [AssociatedValue("ESCALATOR")]
    Escalator,
    
    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}