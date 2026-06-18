using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace DotBahn.Common.Utilities;

/// <summary>
/// Helpers for parsing enums to/from their string representation using <see cref="JsonTypeInfo{T}"/> contracts.
/// </summary>
public static class EnumUtil {
    /// <summary>
    /// Deserializes a JSON string value into an enum.
    /// Returns <paramref name="defaultValue"/> when <paramref name="value"/> is null, empty, or not a valid member name.
    /// </summary>
    public static TEnum Parse<TEnum>(string? value, TEnum defaultValue, JsonTypeInfo<TEnum> typeInfo) where TEnum : struct, Enum {
        if (string.IsNullOrEmpty(value)) {
            return defaultValue;
        }

        try {
            return JsonSerializer.Deserialize("\"" + value + "\"", typeInfo);
        } catch (JsonException) {
            return defaultValue;
        }
    }

    /// <summary>
    /// Serializes an enum to its JSON string member name.
    /// Returns <c>null</c> when <paramref name="value"/> is null.
    /// </summary>
    public static string? Format<TEnum>(TEnum? value, JsonTypeInfo<TEnum> typeInfo) where TEnum : struct, Enum {
        if (value is null) {
            return null;
        }

        return JsonSerializer.Serialize(value.GetValueOrDefault(), typeInfo).Trim('"');
    }
}
