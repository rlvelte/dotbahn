namespace DotBahn.Timetable.Models.Base;

/// <summary>
/// Represents a value that has a planned state and an optional changed state, often used for delays or platform changes.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed record ChangedValue<T> where T : notnull {
    /// <summary>
    /// Gets the original planned value.
    /// </summary>
    public T Planned { get; init; } = default!;

    /// <summary>
    /// Gets the changed value if any deviation from the planned value occurred.
    /// </summary>
    public T? Changed { get; init; } = default;
    
    /// <summary>
    /// Gets the current effective value (the changed value if it exists, otherwise the planned value).
    /// </summary>
    public T Current => Changed != null && !EqualityComparer<T?>.Default.Equals(Changed, default) ? Changed : Planned;

    /// <summary>
    /// Gets a value indicating whether the current value differs from the planned value.
    /// </summary>
    public bool HasChanged => Changed != null && !EqualityComparer<T?>.Default.Equals(Changed, default) && !EqualityComparer<T?>.Default.Equals(Changed, Planned);
}
