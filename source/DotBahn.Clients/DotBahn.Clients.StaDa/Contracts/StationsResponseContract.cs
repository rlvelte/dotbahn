using System.Text.Json.Serialization;

namespace DotBahn.Clients.StaDa.Contracts;

/// <summary>
/// Raw structure for the response containing a list of stations.
/// </summary>
public record StationsResponseContract {
    /// <summary>
    /// Gets the offset.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; init; }

    /// <summary>
    /// Gets the limit.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; init; }

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; init; }

    /// <summary>
    /// Gets the list of stations.
    /// </summary>
    [JsonPropertyName("result")]
    public List<StationContract> Stations { get; init; } = [];
}