using DotBahn.Modules.RequestCache.Service.Base;
using Microsoft.Extensions.Caching.Memory;

namespace DotBahn.Modules.RequestCache.Service;

/// <summary>
/// In-memory implementation of the request caching system
/// </summary>
public class RequestCacheProvider(IMemoryCache memoryCache) : IRequestCache {
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
