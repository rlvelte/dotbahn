using System.Text.Json.Serialization;

namespace DotBahn.Clients.Stations.Contracts;

/// <summary>
/// Raw structure for a station in the StaDa API.
/// </summary>
public record StationContract {
    /// <summary>
    /// Gets the station number.
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; init; }

    /// <summary>
    /// Gets the station name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the mailing address.
    /// </summary>
    [JsonPropertyName("mailingAddress")]
    public MailingAddressContract? MailingAddress { get; init; }

    /// <summary>
    /// Gets the regional area.
    /// </summary>
    [JsonPropertyName("regionalBereich")]
    public RegionalAreaContract? RegionalArea { get; init; }

    /// <summary>
    /// Gets the Ril100 identifiers.
    /// </summary>
    [JsonPropertyName("ril100Identifiers")]
    public List<Ril100IdentifierContract> Ril100Identifiers { get; init; } = [];

    /// <summary>
    /// Gets the EVA numbers.
    /// </summary>
    [JsonPropertyName("evaNumbers")]
    public List<EvaNumberContract> EvaNumbers { get; init; } = [];

    /// <summary>
    /// Gets the category.
    /// </summary>
    [JsonPropertyName("category")]
    public int Category { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has parking.
    /// </summary>
    [JsonPropertyName("hasParking")]
    public bool HasParking { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has bicycle parking.
    /// </summary>
    [JsonPropertyName("hasBicycleParking")]
    public bool HasBicycleParking { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has public facilities.
    /// </summary>
    [JsonPropertyName("hasPublicFacilities")]
    public bool HasPublicFacilities { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has a locker system.
    /// </summary>
    [JsonPropertyName("hasLockerSystem")]
    public bool HasLockerSystem { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has step-free access.
    /// </summary>
    [JsonPropertyName("hasStepFreeAccess")]
    public bool HasStepFreeAccess { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has a travel center.
    /// </summary>
    [JsonPropertyName("hasTravelCenter")]
    public bool HasTravelCenter { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has travel necessities.
    /// </summary>
    [JsonPropertyName("hasTravelNecessities")]
    public bool HasTravelNecessities { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has stepless access.
    /// </summary>
    [JsonPropertyName("hasSteplessAccess")]
    public bool HasSteplessAccess { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has mobility service.
    /// </summary>
    [JsonPropertyName("hasMobilityService")]
    public bool HasMobilityService { get; init; }

    /// <summary>
    /// Gets a value indicating whether the station has WiFi.
    /// </summary>
    [JsonPropertyName("hasWiFi")]
    public bool HasWiFi { get; init; }
}