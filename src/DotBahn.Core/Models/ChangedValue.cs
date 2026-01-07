namespace DotBahn.Core.Models;

/// <summary>
/// Represents a value that has a planned state and an optional changed state.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed record ChangedValue<T> {
    /// <summary>
    /// Gets the original planned value.
    /// </summary>
    public T Planned { get; init; } = default!;

    /// <summary>
    /// Gets the changed value if any deviation from the planned value occurred.
    /// </summary>
    public T? Changed { get; init; }
    
    /// <summary>
    /// Gets the current effective value (the changed value if it exists, otherwise the planned value).
    /// </summary>
    public T Current => Changed != null ? Changed : Planned;

    /// <summary>
    /// Gets a value indicating whether the current value differs from the planned value.
    /// </summary>
    public bool HasChanged => Changed != null && !EqualityComparer<T?>.Default.Equals(Changed, Planned);
}
