using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Telemetry.Additional;

namespace DotBahn.Modules.Telemetry.Service;

/// <summary>
/// Decorator for <see cref="ICache"/> that adds telemetry instrumentation.
/// </summary>
/// <param name="inner">The inner cache implementation to decorate.</param>
public class TelemetryCacheDecorator(ICache inner) : ICache {
    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key) {
        if (!DotBahnDiagnostics.ActivitySource.HasListeners()) {
            var result = await inner.GetAsync<T>(key);
            RecordCacheMetrics(result != null);
            return result;
        }

        using var activity = DotBahnDiagnostics.ActivitySource.StartActivity("cache.get");
        activity?.SetTag("cache.key", key);

        var value = await inner.GetAsync<T>(key);
        var isHit = value != null;

        activity?.SetTag("cache.hit", isHit);
        RecordCacheMetrics(isHit);
        
        return value;
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value) {
        if (!DotBahnDiagnostics.ActivitySource.HasListeners()) {
            await inner.SetAsync(key, value);
            return;
        }

        using var activity = DotBahnDiagnostics.ActivitySource.StartActivity("cache.set");
        activity?.SetTag("cache.key", key);
        
        await inner.SetAsync(key, value);
    }

    /// <summary>
    /// Record if the cache was hit or missed.
    /// </summary>
    /// <param name="isHit">If the cache was hit.</param>
    private static void RecordCacheMetrics(bool isHit) {
        if (isHit) {
            DotBahnDiagnostics.CacheHitCounter.Add(1);
        } else {
            DotBahnDiagnostics.CacheMissCounter.Add(1);
        }
    }
}
