using DotBahn.Data.Shared.Enumerations;

namespace DotBahn.Data.Timetables.Enumerations;

/// <summary>
/// Type of trip/train service.
/// </summary>
public enum TripType {
    /// <summary>
    /// Regular passenger service.
    /// </summary>
    [AssociatedValue("p")]
    Passenger,

    /// <summary>
    /// Empty train movement (no passengers).
    /// </summary>
    [AssociatedValue("e")]
    Empty,

    /// <summary>
    /// Additional train type (z).
    /// </summary>
    [AssociatedValue("z")]
    Z,

    /// <summary>
    /// Additional train type (s).
    /// </summary>
    [AssociatedValue("s")]
    S,

    /// <summary>
    /// Additional train type (h).
    /// </summary>
    [AssociatedValue("h")]
    H,

    /// <summary>
    /// Additional train type (n).
    /// </summary>
    [AssociatedValue("n")]
    N,
    
    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
