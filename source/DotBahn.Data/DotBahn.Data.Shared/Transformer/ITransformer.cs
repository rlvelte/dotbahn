namespace DotBahn.Data.Shared.Transformer;

/// <summary>
/// Generic interface for transforming data onto a target type.
/// </summary>
/// <typeparam name="TModel">The type to transform on.</typeparam>
/// <typeparam name="TContract">The type to extract from.</typeparam>
public interface ITransformer<TModel, TContract> {
    /// <summary>
    /// Transform the contract into the domain model.
    /// </summary>
    /// <param name="contract">The contract data to transform.</param>
    /// <returns>The transformed object.</returns>
    TModel Transform(in TContract contract);
    
    /// <summary>
    /// Merges the incoming changes onto the target object.
    /// </summary>
    /// <param name="current">The current data</param>
    /// <param name="changes">The change data to merge on the current.</param>
    /// <returns>The merged object.</returns>
    TModel Merge(TModel current, in TModel changes);
}
