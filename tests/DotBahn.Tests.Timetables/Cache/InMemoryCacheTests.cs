using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Service;

namespace DotBahn.Tests.Timetables.Cache;

public class InMemoryCacheTests {
    private static CacheOptions DefaultOptions() => new() {
        DefaultExpiration = TimeSpan.FromMinutes(5)
    };

    [Fact]
    public async Task SetAsync_ThenGetAsync_ReturnsCachedValue() {
        // Arrange
        using var cache = new InMemoryCache(DefaultOptions());

        // Act
        await cache.SetAsync("key", "value");
        var result = await cache.GetAsync<string>("key");

        // Assert
        Assert.Equal("value", result);
    }

    [Fact]
    public async Task GetAsync_UnknownKey_ReturnsNull() {
        // Arrange
        using var cache = new InMemoryCache(DefaultOptions());

        // Act
        var result = await cache.GetAsync<string>("missing");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithSizeLimit_StoresThenRetrievesEntries() {
        // Arrange
        using var cache = new InMemoryCache(new CacheOptions {
            DefaultExpiration = TimeSpan.FromMinutes(5),
            SizeLimit = 10
        });

        // Act
        await cache.SetAsync("k1", "v1");
        await cache.SetAsync("k2", "v2");

        // Assert
        Assert.Equal("v1", await cache.GetAsync<string>("k1"));
        Assert.Equal("v2", await cache.GetAsync<string>("k2"));
    }

    [Fact]
    public async Task SetAsync_OverwritesExistingKey() {
        // Arrange
        using var cache = new InMemoryCache(DefaultOptions());
        await cache.SetAsync("key", "first");

        // Act
        await cache.SetAsync("key", "second");

        // Assert
        Assert.Equal("second", await cache.GetAsync<string>("key"));
    }

    [Fact]
    public async Task GetAsync_AfterExpiration_ReturnsNull() {
        // Arrange
        using var cache = new InMemoryCache(new CacheOptions {
            DefaultExpiration = TimeSpan.FromMilliseconds(50)
        });
        await cache.SetAsync("key", "value");

        // Act
        await Task.Delay(150);

        // Assert
        Assert.Null(await cache.GetAsync<string>("key"));
    }

    [Fact]
    public void Dispose_DoesNotThrow() {
        // Arrange
        using var cache = new InMemoryCache(DefaultOptions());

        // Act + Assert
        Assert.Null(Record.Exception(() => cache.Dispose()));
    }
}
