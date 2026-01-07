using DotBahn.Modules.Cache.Base;
using Microsoft.Extensions.Caching.Memory;

namespace DotBahn.Modules.Cache;

/// <summary>
/// In-memory implementation of the caching system
/// </summary>
public class InMemoryCacheProvider(IMemoryCache memoryCache) : ICacheProvider {
    private readonly IMemoryCache _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key) {
        return Task.FromResult(_cache.Get<T>(key));
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue) {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }
}
