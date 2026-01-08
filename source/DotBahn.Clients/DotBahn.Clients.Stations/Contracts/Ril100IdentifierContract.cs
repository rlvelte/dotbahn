using System.Text.Json.Serialization;

namespace DotBahn.Clients.Stations.Contracts;

/// <summary>
///Raw structure for a Ril100 identifier.
/// </summary>
public record Ril100IdentifierContract {
    /// <summary>
    /// Gets the Ril identifier.
    /// </summary>
    [JsonPropertyName("rilIdentifier")]
    public string RilIdentifier { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this is the main identifier.
    /// </summary>
    [JsonPropertyName("isMain")]
    public bool IsMain { get; init; }
}