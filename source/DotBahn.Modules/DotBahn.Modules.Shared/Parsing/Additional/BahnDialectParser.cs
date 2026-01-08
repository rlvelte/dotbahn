using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotBahn.Modules.Shared.Parsing.Additional;

/// <summary>
/// Converts dialect in the contracts to real types (e.g. 'yes' as 'true').
/// </summary>
public sealed class BahnDialectJsonConverter : JsonConverter<bool> {
    /// <inheritdoc />
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.True) {
            return true;
        }

        if (reader.TokenType == JsonTokenType.False) {
            return false;
        }

        if (reader.TokenType == JsonTokenType.String) {
            var value = reader.GetString()?.Trim().ToLowerInvariant();

            return value switch {
                "true" => true,
                "false" => false,
                "yes" => true,
                "no" => false,
                "1" => true,
                _ => false
            };
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing boolean.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) {
        writer.WriteBooleanValue(value);
    }
}