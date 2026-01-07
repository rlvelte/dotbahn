namespace DotBahn.TimetableApi.Enumerations;

/// <summary>
/// Defines the type of trip a train is performing.
/// </summary>
public enum TripType {
    /// <summary>
    /// A standard passenger train.
    /// </summary>
    Passenger,

    /// <summary>
    /// An empty train run (e.g., for positioning).
    /// </summary>
    Empty,

    /// <summary>
    /// A freight train.
    /// </summary>
    Freight
}