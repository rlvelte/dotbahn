namespace DotBahn.Cache.Configuration;

/// <summary>
/// Configuration options for the caching system
/// </summary>
public record CacheOptions {
    /// <summary>
    /// Default expiration time for general requests (e.g., 30 seconds)
    /// </summary>
    public TimeSpan DefaultExpiration { get; init; } = TimeSpan.FromSeconds(30);
    /// <summary>
    /// Path-specific expiration overrides
    /// </summary>
    public Dictionary<string, TimeSpan> PathExpirations { get; init; } = new();
}
