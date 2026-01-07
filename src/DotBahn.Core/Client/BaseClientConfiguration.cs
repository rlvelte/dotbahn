using DotBahn.Cache.Configuration;

namespace DotBahn.Core.Client;

/// <summary>
/// Configuration for DotBahn API
/// </summary>
public record BaseClientConfiguration {
    /// <summary>
    /// The base url of the endpoint
    /// </summary>
    public required string BaseUrl { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }

    /// <summary>
    /// Caching configuration
    /// </summary>
    public CacheOptions CacheOptions { get; init; } = new();
}