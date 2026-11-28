namespace DotBahn.Boards.Models.Operators;

/// <summary>
/// Administration and operator information for a transport.
/// </summary>
public class Administration {
    /// <summary>
    /// Identifier of the administration.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Code of the operator.
    /// </summary>
    public required string OperatorCode { get; init; }

    /// <summary>
    /// Name of the operator.
    /// </summary>
    public required string OperatorName { get; init; }

    /// <summary>
    /// Localized transport company information.
    /// </summary>
    public IReadOnlyList<TransportCompany> TransportCompanies { get; init; } = [];
}
