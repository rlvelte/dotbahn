namespace DotDB.Models.Raw;

/// <summary>
/// Raw attributes for a stop (train stop at station)
/// </summary>
public record RawStop {
    /// <summary>
    /// Unique stop ID
    /// </summary>
    public string Id { get; init; }
    
    /// <summary>
    /// EVA number of the station
    /// </summary>
    public string Eva { get; init; }
}