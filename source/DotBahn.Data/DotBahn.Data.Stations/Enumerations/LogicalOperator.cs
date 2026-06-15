using System.Text.Json.Serialization;

namespace DotBahn.Data.Stations.Enumerations;

/// <summary>
/// Represents the logical operator to combine multiple filter criteria in a station query.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<LogicalOperator>))]
public enum LogicalOperator {
    /// <summary>
    /// Logical AND operator. All filter conditions must be true.
    /// </summary>
    [JsonStringEnumMemberName("AND")]
    And,

    /// <summary>
    /// Logical OR operator. At least one filter condition must be true.
    /// </summary>
    [JsonStringEnumMemberName("OR")]
    Or
}
