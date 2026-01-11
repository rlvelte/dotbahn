namespace DotBahn.Modules.Cache.Service.Base;

/// <summary>
/// No-op implementation of the caching system (Null Object Pattern).
/// </summary>
public class NullCache : ICache {
    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key) {
        return Task.FromResult(default(T));
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value) {
        return Task.CompletedTask;
    }
}
