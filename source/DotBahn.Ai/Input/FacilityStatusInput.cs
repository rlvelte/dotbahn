using System.ComponentModel;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Ai.Input;

/// <summary>
/// AI optimized input DTO for the facility status tool.
/// </summary>
public sealed record FacilityStatusInput {
    /// <summary>
    /// The station number whose facilities should be queried.
    /// </summary>
    [Description("DB station number returned by search_stations.")]
    public required int StationNumber { get; init; }

    /// <summary>
    /// Only return facilities of this type.
    /// </summary>
    [Description("Only return facilities of this type.")]
    public FacilityType? Type { get; init; }

    /// <summary>
    /// Only return facilities in this state.
    /// </summary>
    [Description("Only return facilities in this state.")]
    public FacilityState? State { get; init; }

    /// <summary>
    /// Maximum number of facilities to return.
    /// </summary>
    [Description("Maximum number of facilities to return (1-25).")]
    public int MaxResults { get; init; } = 25;

    /// <summary>
    /// Validates that the DTO constraints are respected by the LLM.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when a required value is missing or invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a value is outside the allowed range.</exception>
    internal void Validate() {
        if (StationNumber <= 0) {
            throw new ArgumentOutOfRangeException(nameof(StationNumber), "StationNumber must be positive.");
        }

        if (MaxResults is < 1 or > 25) {
            throw new ArgumentOutOfRangeException(nameof(MaxResults), "MaxResults must be between 1 and 25.");
        }
    }
}
