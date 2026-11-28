using System.ComponentModel;

namespace DotBahn.Ai.Input;

/// <summary>
/// AI optimized input DTO for the arrivals and departures tools.
/// </summary>
public record BoardToolInput {
    /// <summary>
    /// Identifier returned by the station search tool.
    /// </summary>
    [Description("Identifier returned by search_stations, e.g. an EVA number.")]
    public required string StationId { get; init; }

    /// <summary>
    /// Identifier scheme used by <see cref="StationId"/>.
    /// </summary>
    [Description("Identifier scheme: Eva, Stada, or Dhid.")]
    public required StationIdentifierKind StationKind { get; init; }

    /// <summary>
    /// Optional start of the time window in ISO 8601 format.
    /// </summary>
    [Description("Optional start of the time window in ISO 8601 format. Defaults to now.")]
    public DateTime? Start { get; init; }

    /// <summary>
    /// Maximum number of board entries to return.
    /// </summary>
    [Description("Maximum number of board entries to return (1-25).")]
    public int MaxEntries { get; init; } = 10;

    /// <summary>
    /// Validates that the DTO constraints are respected by the LLM.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when a required value is missing or invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a value is outside the allowed range.</exception>
    internal void Validate() {
        if (string.IsNullOrWhiteSpace(StationId) || StationId.Length > 64) {
            throw new ArgumentException("StationId must contain between 1 and 64 characters.", nameof(StationId));
        }

        if (!Enum.IsDefined(StationKind)) {
            throw new ArgumentOutOfRangeException(nameof(StationKind), "StationKind must be Eva, Stada, or Dhid.");
        }

        if (MaxEntries is < 1 or > 25) {
            throw new ArgumentOutOfRangeException(nameof(MaxEntries), "MaxEntries must be between 1 and 25.");
        }
    }
}
