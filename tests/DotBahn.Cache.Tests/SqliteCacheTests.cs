using FluentAssertions;

namespace DotBahn.Cache.Tests;

public class SqliteCacheTests : IDisposable {
    private readonly string _dbPath;

    public SqliteCacheTests() {
        _dbPath = $"test_cache_{Guid.NewGuid()}.db";
    }

    [Fact]
    public async Task SetAndGet_ShouldReturnCorrectValue() {
        // Arrange
        using var cache = new SqliteCache(_dbPath);
        var key = "testKey";
        var value = "testValue";

        // Act
        await cache.SetAsync(key, value);
        var result = await cache.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task Get_ShouldReturnNull_WhenExpired() {
        // Arrange
        using var cache = new SqliteCache(_dbPath);
        var key = "expiredKey";
        var value = "expiredValue";

        // Act
        await cache.SetAsync(key, value, TimeSpan.FromMilliseconds(1));
        await Task.Delay(10); // Wait for expiration
        var result = await cache.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Set_ShouldOverwriteExistingValue() {
        // Arrange
        using var cache = new SqliteCache(_dbPath);
        var key = "key";

        // Act
        await cache.SetAsync(key, "value1");
        await cache.SetAsync(key, "value2");
        var result = await cache.GetAsync<string>(key);

        // Assert
        result.Should().Be("value2");
    }

    [Fact]
    public async Task SetAndGet_ComplexObject_ShouldWork() {
        // Arrange
        using var cache = new SqliteCache(_dbPath);
        var key = "complex";
        var value = new { Name = "Test", Id = 123 };

        // Act
        await cache.SetAsync(key, value);
        var result = await cache.GetAsync<TestObject>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(value.Name);
        result.Id.Should().Be(value.Id);
    }

    public void Dispose() {
        if (File.Exists(_dbPath)) {
            try {
                File.Delete(_dbPath);
            } catch {
                // Ignore
            }
        }
    }

    private class TestObject {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
