using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Modules.Cache;

/// <summary>
/// Configuration options for the caching system
/// </summary>
[ExcludeFromCodeCoverage]
public record CacheOptions {
    /// <summary>
    /// Default expiration time for general requests.
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; }

    /// <summary>
    /// Maximum number of entries the cache may hold. Each entry counts as 1 unit.
    /// Null (default) means unbounded.
    /// </summary>
    public long? SizeLimit { get; set; }
}
