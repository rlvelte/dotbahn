using System.Text.Json.Serialization;

namespace DotBahn.Clients.FaSta.Contracts;

/// <summary>
/// Raw structure for a station in the FaSta API.
/// </summary>
public record StationContract {
    /// <summary>
    /// Gets the station number.
    /// </summary>
    [JsonPropertyName("stationnumber")]
    public int StationNumber { get; init; }

    /// <summary>
    /// Gets the station name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the facilities at this station.
    /// </summary>
    [JsonPropertyName("facilities")]
    public List<FacilityContract> Facilities { get; init; } = [];
}
