namespace DotBahn.Parsing.Base;

/// <summary>
/// Generic interface for transforming a raw type into a domain model.
/// </summary>
/// <typeparam name="TContract">The raw input type.</typeparam>
/// <typeparam name="TModel">The target domain model type.</typeparam>
public interface ITransformer<in TContract, out TModel> {
    /// <summary>
    /// Transforms the raw data into the domain model.
    /// </summary>
    /// <param name="contract">The raw data.</param>
    /// <returns>The domain model.</returns>
    TModel Transform(TContract contract);
}
