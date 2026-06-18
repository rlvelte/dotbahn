using System.ComponentModel;

namespace DotBahn.Shared.Transformer;

/// <summary>
/// Generic interface for transforming data into a target type.
/// </summary>
/// <typeparam name="TModel">The type to transform to.</typeparam>
/// <typeparam name="TContract">The type to extract from.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ITransformer<out TModel, TContract> {
    /// <summary>
    /// Transform the contract into the domain model.
    /// </summary>
    /// <param name="contracts">The contract data to transform.</param>
    /// <returns>The transformed object.</returns>
    TModel Transform(in TContract contracts);
}
