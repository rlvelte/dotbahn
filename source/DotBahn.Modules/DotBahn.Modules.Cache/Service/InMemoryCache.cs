using DotBahn.Modules.Cache.Service.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DotBahn.Modules.Cache.Service;

/// <summary>
/// In-memory implementation of the caching system with structured logging
/// </summary>
/// <param name="cache">The memory cache to use.</param>
/// <param name="options">Options for the cache.</param>
/// <param name="logger">Logger for debug and trace information.</param>
public class InMemoryCache(IMemoryCache cache, CacheOptions options, ILogger<InMemoryCache>? logger = null) : ICache {
    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key) {
        if (cache.TryGetValue<T>(key, out var value)) {
            if (logger != null && logger.IsEnabled(LogLevel.Debug)) {
                logger.LogDebug("[InMemoryCache] Cache hit for '{Key}'.", key);
            }
            return Task.FromResult(value);
        } else {
            if (logger != null && logger.IsEnabled(LogLevel.Debug)) {
                logger.LogDebug("[InMemoryCache] Cache miss for '{Key}'.", key);
            }
            return Task.FromResult<T?>(default);
        }
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value) {
        cache.Set(key, value, new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = options.DefaultExpiration
        });

        if (logger != null && logger.IsEnabled(LogLevel.Debug)) {
            logger.LogDebug("[InMemoryCache] Set key '{Key}' with expiration {ExpirationSeconds}s.", key, options.DefaultExpiration.TotalSeconds);
        }

        return Task.CompletedTask;
    }
}