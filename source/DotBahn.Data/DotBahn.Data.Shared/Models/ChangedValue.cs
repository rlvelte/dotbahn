namespace DotBahn.Data.Shared.Models;

/// <summary>
/// Represents a value type that can be updated, tracking both the original and changed value.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class ChangedValue<T> where T : struct {
    /// <summary>
    /// The original value.
    /// </summary>
    public required T Original { get; init; }
    
    /// <summary>
    /// The updated value, if available.
    /// </summary>
    public T? Updated { get; set; }

    /// <summary>
    /// Indicates if there is an updated value.
    /// </summary>
    public bool HasUpdate => Updated.HasValue;
    
    /// <summary>
    /// Gets the currently active value.
    /// </summary>
    public T Actual => Updated ?? Original;
}