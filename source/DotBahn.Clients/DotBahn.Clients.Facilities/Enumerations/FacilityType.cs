using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Clients.Facilities.Enumerations;

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
    Escalator
}