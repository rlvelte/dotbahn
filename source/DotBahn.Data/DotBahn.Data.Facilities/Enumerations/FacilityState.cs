

using DotBahn.Data.Shared.Enumerations;

namespace DotBahn.Data.Facilities.Enumerations;

/// <summary>
/// Represents the operational state of a facility.
/// </summary>
public enum FacilityState {
    /// <summary>
    /// Facility is active and operational.
    /// </summary>
    [AssociatedValue("ACTIVE")]
    Active,
    
    /// <summary>
    /// Facility is inactive or out of service.
    /// </summary>
    [AssociatedValue("INACTIVE")]
    Inactive,
    
    /// <summary>
    /// Facility state is unknown or not specified.
    /// </summary>
    [AssociatedValue("UNKNOWN")]
    Unknown
}