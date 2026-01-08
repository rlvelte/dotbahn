using System.Text.Json.Serialization;

namespace DotBahn.Clients.Stations.Contracts;

/// <summary>
/// Raw structure for a regional area of the Deutsche Bahn.
/// </summary>
public record RegionalAreaContract {
    /// <summary>
    /// Gets the number of the regional area.
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; init; }

    /// <summary>
    /// Gets the name of the regional area.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the short name of the regional area.
    /// </summary>
    [JsonPropertyName("shortName")]
    public string ShortName { get; init; } = string.Empty;
}