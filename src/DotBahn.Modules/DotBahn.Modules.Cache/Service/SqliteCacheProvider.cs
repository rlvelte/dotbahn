using System.Text.Json;
using DotBahn.Modules.Cache.Base;
using Microsoft.Data.Sqlite;

namespace DotBahn.Modules.Cache;

/// <summary>
/// SQLite implementation of the caching system for local persistence.
/// </summary>
public class SqliteCacheProvider(string databasePath = "dotbahn_cache.db") : ICacheProvider, IDisposable {
    private readonly string _connectionString = $"Data Source={databasePath}";
    private readonly Lock _lock = new();
    private bool _initialized;

    private void EnsureInitialized() {
        lock (_lock) {
            if (_initialized) return;

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS CacheEntries (
                    Key TEXT PRIMARY KEY,
                    Value TEXT,
                    ExpirationTicks INTEGER
                )";
            command.ExecuteNonQuery();

            _initialized = true;
        }
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key) {
        EnsureInitialized();

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Value, ExpirationTicks FROM CacheEntries WHERE Key = $key";
        command.Parameters.AddWithValue("$key", key);

        await using var reader = await command.ExecuteReaderAsync();
        var isExpired = false;
        string? value = null;
        if (await reader.ReadAsync()) {
            value = reader.GetString(0);
            var expirationTicks = reader.GetInt64(1);

            if (expirationTicks > 0 && expirationTicks < DateTime.UtcNow.Ticks) {
                isExpired = true;
            }
        }
        
        await reader.CloseAsync();
        await connection.CloseAsync();

        if (isExpired) {
            await RemoveAsync(key);
            return default;
        }

        if (value != null) {
            return typeof(T) == typeof(string) 
                ? (T)(object)value 
                : JsonSerializer.Deserialize<T>(value);
        }

        return default;
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) {
        EnsureInitialized();

        var jsonValue = typeof(T) == typeof(string) ? value?.ToString() : JsonSerializer.Serialize(value);
        var expirationTicks = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value).Ticks : 0;

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR REPLACE INTO CacheEntries (Key, Value, ExpirationTicks)
            VALUES ($key, $value, $expiration)";
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", jsonValue ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$expiration", expirationTicks);

        await command.ExecuteNonQueryAsync();
    }

    private async Task RemoveAsync(string key) {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM CacheEntries WHERE Key = $key";
        command.Parameters.AddWithValue("$key", key);

        await command.ExecuteNonQueryAsync();
    }

    public void Dispose() { }
}
