using System.ComponentModel;

namespace DotBahn.Ai.Input;

/// <summary>
/// AI optimized input DTO for the station search tool.
/// </summary>
public sealed record StationSearchInput {
    /// <summary>
    /// Station name or fragment to search for.
    /// </summary>
    [Description("Station name or fragment to search for. Supports * and ? wildcards.")]
    public required string Query { get; init; }

    /// <summary>
    /// Maximum number of stations to return.
    /// </summary>
    [Description("Maximum number of stations to return (1-20).")]
    public int MaxResults { get; init; } = 20;

    /// <summary>
    /// Validates that the DTO constraints are respected by the LLM.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when a required value is missing or invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a value is outside the allowed range.</exception>
    internal void Validate() {
        if (string.IsNullOrWhiteSpace(Query) || Query.Length > 200) {
            throw new ArgumentException("Query must contain between 1 and 200 characters.", nameof(Query));
        }

        if (MaxResults is < 1 or > 20) {
            throw new ArgumentOutOfRangeException(nameof(MaxResults), "MaxResults must be between 1 and 20.");
        }
    }
}
