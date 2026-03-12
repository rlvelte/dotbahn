using DotBahn.Modules.Cache.Service.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DotBahn.Modules.Cache.Service;

/// <summary>
/// In-memory implementation of the caching system with structured logging.
/// Owns and manages its own <see cref="MemoryCache"/> instance.
/// </summary>
public sealed class InMemoryCache : ICache {
    private readonly MemoryCache _cache;
    private readonly CacheOptions _options;
    private readonly ILogger<InMemoryCache>? _logger;

    /// <param name="options">Options for the cache.</param>
    /// <param name="logger">Logger for debug and trace information.</param>
    public InMemoryCache(CacheOptions options, ILogger<InMemoryCache>? logger = null) {
        _options = options;
        _logger = logger;
        _cache = new MemoryCache(new MemoryCacheOptions {
            SizeLimit = options.SizeLimit
        });
    }

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key) {
        if (_cache.TryGetValue<T>(key, out var value)) {
            if (_logger != null && _logger.IsEnabled(LogLevel.Debug)) {
                _logger.LogDebug("[InMemoryCache] Cache hit for '{Key}'.", key);
            }
            return Task.FromResult(value);
        }

        if (_logger != null && _logger.IsEnabled(LogLevel.Debug)) {
            _logger.LogDebug("[InMemoryCache] Cache miss for '{Key}'.", key);
        }
        return Task.FromResult<T?>(default);
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value) {
        var entryOptions = new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = _options.DefaultExpiration
        };

        if (_options.SizeLimit.HasValue) {
            entryOptions.Size = 1;
        }

        _cache.Set(key, value, entryOptions);

        if (_logger != null && _logger.IsEnabled(LogLevel.Debug)) {
            _logger.LogDebug("[InMemoryCache] Set key '{Key}' with expiration {ExpirationSeconds}s.", key, _options.DefaultExpiration.TotalSeconds);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose() => _cache.Dispose();
}
