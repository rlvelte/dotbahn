namespace DotBahn.Modules.Shared.Transformer;

/// <summary>
/// Generic interface for transforming a contract type into a domain model.
/// </summary>
/// <typeparam name="TContract">The contract input type.</typeparam>
/// <typeparam name="TModel">The target domain model type.</typeparam>
public interface ITransformer<in TContract, out TModel> {
    /// <summary>
    /// Transforms the contract data into the domain model.
    /// </summary>
    /// <param name="contract">The contract data.</param>
    /// <returns>The domain model.</returns>
    TModel Transform(TContract contract);
}
