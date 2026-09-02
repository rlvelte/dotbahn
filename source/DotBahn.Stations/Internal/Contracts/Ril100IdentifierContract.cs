using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Stations.Internal.Contracts;

/// <summary>
///Raw structure for a Ril100 identifier
/// </summary>
[ExcludeFromCodeCoverage]
internal record Ril100IdentifierContract {
    /// <summary>
    /// Gets the Ril identifier
    /// </summary>
    [JsonPropertyName("rilIdentifier")]
    public string RilIdentifier { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this is the main identifier
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("isMain")]
    public bool IsMain { get; init; }
}
