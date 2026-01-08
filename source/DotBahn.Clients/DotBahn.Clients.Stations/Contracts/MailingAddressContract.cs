using System.Text.Json.Serialization;

namespace DotBahn.Clients.Stations.Contracts;

/// <summary>
/// Raw structure for the mailing address of a station.
/// </summary>
public record MailingAddressContract {
    /// <summary>
    /// Gets the city.
    /// </summary>
    [JsonPropertyName("city")]
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Gets the zip code.
    /// </summary>
    [JsonPropertyName("zipcode")]
    public string ZipCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the street.
    /// </summary>
    [JsonPropertyName("street")]
    public string Street { get; init; } = string.Empty;
}