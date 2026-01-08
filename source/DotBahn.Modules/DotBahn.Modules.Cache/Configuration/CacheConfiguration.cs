using System.Diagnostics.CodeAnalysis;
using DotBahn.Modules.Cache.Enumerations;
using JetBrains.Annotations;

namespace DotBahn.Modules.Cache.Configuration;

/// <summary>
/// Configuration options for the caching system
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record CacheConfiguration {
    /// <summary>
    /// The type of cache provider to use
    /// </summary>
    public CacheProviderType ProviderType { get; set; } = CacheProviderType.InMemory;

    /// <summary>
    /// Path to the SQLite database file (only used if ProviderType is Sqlite)
    /// </summary>
    public string SqliteDatabasePath { get; init; } = "dotbahn_cache.db";

    /// <summary>
    /// Default expiration time for general requests (e.g., 30 seconds)
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromSeconds(30);
}
