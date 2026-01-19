namespace DotBahn.Data.Shared.Transformer;

/// <summary>
/// Generic interface for merging data onto a target type.
/// </summary>
/// <typeparam name="TModel">The type to transform on.</typeparam>
public interface IMerger<TModel> {
    /// <summary>
    /// Merges the incoming changes onto the target object.
    /// </summary>
    /// <param name="current">The current data</param>
    /// <param name="changes">The change data to merge on the current.</param>
    /// <returns>The merged object.</returns>
    TModel Merge(TModel current, in TModel changes);
}