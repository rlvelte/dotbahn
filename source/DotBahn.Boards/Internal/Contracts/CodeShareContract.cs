using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a codeshare.
/// </summary>
[ExcludeFromCodeCoverage]
internal record CodeShareContract {
    /// <summary>
    /// Gets the airline code.
    /// </summary>
    [JsonPropertyName("airlineCode")]
    public string AirlineCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the flight number.
    /// </summary>
    [JsonPropertyName("flightnumber")]
    public int Flightnumber { get; init; }
}
