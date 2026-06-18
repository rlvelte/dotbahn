namespace DotBahn.Modules.Cache;

/// <summary>
/// No-op implementation of the caching system (Null Object Pattern).
/// </summary>
public sealed class NullCache : ICache {
    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key) => Task.FromResult(default(T));

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value) => Task.CompletedTask;

    /// <inheritdoc />
    public void Dispose() { }
}
