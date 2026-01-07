namespace DotBahn.Modules.Cache.Configuration;

/// <summary>
/// Configuration options for the caching system
/// </summary>
public record CacheConfiguration {
    /// <summary>
    /// Default expiration time for general requests (e.g., 30 seconds)
    /// </summary>
    public TimeSpan DefaultExpiration { get; init; } = TimeSpan.FromSeconds(30);
}
