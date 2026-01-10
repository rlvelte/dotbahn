using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Modules.Cache;

/// <summary>
/// Configuration options for the caching system
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record CacheOptions {
    /// <summary>
    /// Default expiration time for general requests.
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; }
}
