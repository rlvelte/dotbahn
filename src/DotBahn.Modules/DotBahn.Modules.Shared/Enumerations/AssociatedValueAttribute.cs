using JetBrains.Annotations;

namespace DotBahn.Modules.Shared.Enumerations;

/// <summary>
/// Attribute to associate a string value to an enum member.
/// </summary>
/// <param name="value">The string value from the API.</param>
[AttributeUsage(AttributeTargets.Field)]
[UsedImplicitly]
public sealed class AssociatedValueAttribute(string value) : Attribute {
    /// <summary>
    /// Gets the mapped string value.
    /// </summary>
    public string Value { get; } = value;
}