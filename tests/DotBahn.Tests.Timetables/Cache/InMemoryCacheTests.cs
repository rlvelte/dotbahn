using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Service;

namespace DotBahn.Tests.Timetables.Cache;

public class InMemoryCacheTests {
    private static CacheOptions DefaultOptions() => new() {
        DefaultExpiration = TimeSpan.FromMinutes(5)
    };

    [Fact]
    public async Task SetAsync_ThenGetAsync_ReturnsCachedValue() {
        using var cache = new InMemoryCache(DefaultOptions());

        await cache.SetAsync("key", "value");
        var result = await cache.GetAsync<string>("key");

        Assert.Equal("value", result);
    }

    [Fact]
    public async Task GetAsync_UnknownKey_ReturnsNull() {
        using var cache = new InMemoryCache(DefaultOptions());

        var result = await cache.GetAsync<string>("missing");

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithSizeLimit_StoresThenRetrievesEntries() {
        using var cache = new InMemoryCache(new CacheOptions {
            DefaultExpiration = TimeSpan.FromMinutes(5),
            SizeLimit = 10
        });

        await cache.SetAsync("k1", "v1");
        await cache.SetAsync("k2", "v2");

        Assert.Equal("v1", await cache.GetAsync<string>("k1"));
        Assert.Equal("v2", await cache.GetAsync<string>("k2"));
    }

    [Fact]
    public async Task SetAsync_OverwritesExistingKey() {
        using var cache = new InMemoryCache(DefaultOptions());
        await cache.SetAsync("key", "first");

        await cache.SetAsync("key", "second");

        Assert.Equal("second", await cache.GetAsync<string>("key"));
    }

    [Fact]
    public async Task GetAsync_AfterExpiration_ReturnsNull() {
        using var cache = new InMemoryCache(new CacheOptions {
            DefaultExpiration = TimeSpan.FromMilliseconds(50)
        });
        await cache.SetAsync("key", "value");

        await Task.Delay(150);

        Assert.Null(await cache.GetAsync<string>("key"));
    }

    [Fact]
    public void Dispose_DoesNotThrow() {
        using var cache = new InMemoryCache(DefaultOptions());

        Assert.Null(Record.Exception(cache.Dispose));
    }

    [Fact]
    public async Task Cache_AfterConstruction_IsUsable() {
        using var cache = new InMemoryCache(DefaultOptions());

        var result = await cache.GetAsync<string>("any-key");

        Assert.Null(result);
    }

    [Fact]
    public async Task Cache_WithSizeLimit_AcceptsEntriesUpToLimit() {
        using var cache = new InMemoryCache(new CacheOptions {
            DefaultExpiration = TimeSpan.FromMinutes(5),
            SizeLimit = 3
        });

        await cache.SetAsync("a", "alpha");
        await cache.SetAsync("b", "beta");
        await cache.SetAsync("c", "gamma");

        Assert.Equal("alpha", await cache.GetAsync<string>("a"));
        Assert.Equal("beta", await cache.GetAsync<string>("b"));
        Assert.Equal("gamma", await cache.GetAsync<string>("c"));
    }
}
