namespace DotBahn.Data.Shared.Transformer;

/// <summary>
/// Generic interface for transforming data into a target type.
/// </summary>
/// <typeparam name="TModel">The type to transform to.</typeparam>
/// <typeparam name="TContract">The type to extract from.</typeparam>
public interface ITransformer<out TModel, TContract> {
    /// <summary>
    /// Transform the contract into the domain model.
    /// </summary>
    /// <param name="contract">The contract data to transform.</param>
    /// <returns>The transformed object.</returns>
    TModel Transform(in TContract contract);
}