namespace DotBahn.Data.Shared.Models;

/// <summary>
/// Represents a reference type that can be updated, tracking both the original and changed value.
/// </summary>
/// <typeparam name="T">The reference type.</typeparam>
public class ChangedRef<T> where T : class {
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
    public bool HasUpdate => Updated != null;
    
    /// <summary>
    /// Gets the currently active value.
    /// </summary>
    public T Actual => Updated ?? Original;
}