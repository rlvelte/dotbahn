using System.Text.Json.Serialization;

namespace DotBahn.Clients.FaSta.Contracts;

/// <summary>
/// Raw structure for a station facility (elevator or escalator) in the FaSta API.
/// </summary>
public record FacilityContract {
    /// <summary>
    /// Gets the unique equipment number.
    /// </summary>
    [JsonPropertyName("equipmentnumber")]
    public long EquipmentNumber { get; init; }

    /// <summary>
    /// Gets the type of the facility (e.g., ELEVATOR, ESCALATOR).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the description of the facility.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the current state of the facility (e.g., ACTIVE, INACTIVE, UNKNOWN).
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>
    /// Gets an explanation for the current state.
    /// </summary>
    [JsonPropertyName("stateExplanation")]
    public string? StateExplanation { get; init; }

    /// <summary>
    /// Gets the station number this facility belongs to.
    /// </summary>
    [JsonPropertyName("stationnumber")]
    public int StationNumber { get; init; }

    /// <summary>
    /// Gets the longitude (X coordinate).
    /// </summary>
    [JsonPropertyName("geocoordX")]
    public double? Longitude { get; init; }

    /// <summary>
    /// Gets the latitude (Y coordinate).
    /// </summary>
    [JsonPropertyName("geocoordY")]
    public double? Latitude { get; init; }

    /// <summary>
    /// Gets the name of the operator.
    /// </summary>
    [JsonPropertyName("operatorname")]
    public string? OperatorName { get; init; }
}
