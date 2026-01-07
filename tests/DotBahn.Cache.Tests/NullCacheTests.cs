using FluentAssertions;

namespace DotBahn.Cache.Tests;

public class NullCacheTests {
    [Fact]
    public async Task GetAsync_ShouldAlwaysReturnNull() {
        // Arrange
        var cache = new NullCache();
        await cache.SetAsync("test", "value");

        // Act
        var result = await cache.GetAsync<string>("test");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldNotThrow() {
        // Arrange
        var cache = new NullCache();

        // Act & Assert
        await cache.Awaiting(c => c.SetAsync("test", "value")).Should().NotThrowAsync();
    }
}
