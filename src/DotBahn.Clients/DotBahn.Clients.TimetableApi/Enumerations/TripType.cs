using DotBahn.Modules.Shared;
using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.TimetableApi.Enumerations;

/// <summary>
/// Defines the type of trip a train is performing.
/// </summary>
public enum TripType {
    /// <summary>
    /// A standard passenger train.
    /// </summary>
    [AssociatedValue("p")]
    Passenger,

    /// <summary>
    /// An empty train run (e.g., for positioning).
    /// </summary>
    [AssociatedValue("e")]
    Empty,

    /// <summary>
    /// A freight train.
    /// </summary>
    [AssociatedValue("z")]
    Freight
}