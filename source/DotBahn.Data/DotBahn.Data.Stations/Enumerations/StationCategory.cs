namespace DotBahn.Data.Stations.Enumerations;

/// <summary>
/// Category of a railway station based on its importance and traffic volume.
/// Categories range from 1 (major hub) to 7 (minor stop).
/// </summary>
public enum StationCategory {
    /// <summary>
    /// Major transportation hub with highest passenger traffic.
    /// </summary>
    Category1 = 1,

    /// <summary>
    /// Large station with significant regional and long-distance traffic.
    /// </summary>
    Category2 = 2,

    /// <summary>
    /// Important regional station with moderate traffic.
    /// </summary>
    Category3 = 3,

    /// <summary>
    /// Larger regional station.
    /// </summary>
    Category4 = 4,

    /// <summary>
    /// Standard station with basic services.
    /// </summary>
    Category5 = 5,

    /// <summary>
    /// Small station with limited services.
    /// </summary>
    Category6 = 6,

    /// <summary>
    /// Minor stop with minimal infrastructure.
    /// </summary>
    Category7 = 7,
    
    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
