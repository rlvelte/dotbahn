using System.Reflection;

namespace DotBahn.Data.Shared.Enumerations;

/// <summary>
/// Extension methods for enumerations.
/// </summary>
public static class EnumExtensions {
    /// <summary>
    /// Parses a string value into an enum member based on the <see cref="AssociatedValueAttribute"/>.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    /// <param name="value">The string value to parse.</param>
    /// <param name="defaultValue">The default value to return if no match is found.</param>
    /// <returns>The matched enum member or the default value.</returns>
    public static TEnum FromAssociatedValue<TEnum>(string? value, TEnum defaultValue) where TEnum : struct, Enum {
        if (string.IsNullOrEmpty(value)) {
            return defaultValue;
        }

        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)) {
            var attribute = field.GetCustomAttribute<AssociatedValueAttribute>();
            if (attribute != null && attribute.Value.Equals(value, StringComparison.OrdinalIgnoreCase)) {
                return (TEnum)field.GetValue(null)!;
            }
        }

        return defaultValue;
    }
    
    /// <summary>
    /// Gets the value of the <see cref="AssociatedValueAttribute"/> for a given enum member.
    /// </summary>
    /// <param name="enumValue">The enum member to retrieve the associated value from.</param>
    /// <returns>The associated value if present; otherwise, <c>null</c>.</returns>
    public static string? GetAssociatedValue(this Enum enumValue) {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = field?.GetCustomAttribute<AssociatedValueAttribute>();
        return attribute?.Value;
    }
}