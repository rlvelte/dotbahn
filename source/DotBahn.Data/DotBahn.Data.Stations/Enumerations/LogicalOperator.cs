using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Data.Stations.Enumerations;

/// <summary>
/// Represents the logical operator to combine multiple filter criteria in a station query.
/// </summary>
public enum LogicalOperator {
    /// <summary>
    /// Logical AND operator. All filter conditions must be true.
    /// Associated value: "AND".
    /// </summary>
    [AssociatedValue("AND")]
    And,

    /// <summary>
    /// Logical OR operator. At least one filter condition must be true.
    /// Associated value: "OR".
    /// </summary>
    [AssociatedValue("OR")]
    Or,
}