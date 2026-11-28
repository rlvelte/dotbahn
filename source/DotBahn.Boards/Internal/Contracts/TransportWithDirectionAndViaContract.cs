using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a transport with direction and via stops.
/// </summary>
[ExcludeFromCodeCoverage]
internal record TransportWithDirectionAndViaContract {
    /// <summary>
    /// Gets the administration identifier.
    /// </summary>
    [JsonPropertyName("administrationId")]
    public string AdministrationId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the category.
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Gets the internal category.
    /// </summary>
    [JsonPropertyName("categoryInternal")]
    public string CategoryInternal { get; init; } = string.Empty;

    /// <summary>
    /// Gets the DFID.
    /// </summary>
    [JsonPropertyName("dfid")]
    public string? Dfid { get; init; }

    /// <summary>
    /// Gets the direction.
    /// </summary>
    [JsonPropertyName("direction")]
    public DirectionInfoContract? Direction { get; init; }

    /// <summary>
    /// Gets the DTID.
    /// </summary>
    [JsonPropertyName("dtid")]
    public string? Dtid { get; init; }

    /// <summary>
    /// Gets the journey description.
    /// </summary>
    [JsonPropertyName("journeyDescription")]
    public string JourneyDescription { get; init; } = string.Empty;

    /// <summary>
    /// Gets the journey number.
    /// </summary>
    [JsonPropertyName("journeyNumber")]
    public int JourneyNumber { get; init; }

    /// <summary>
    /// Gets the label.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; init; }

    /// <summary>
    /// Gets the line.
    /// </summary>
    [JsonPropertyName("line")]
    public LineInfoContract? Line { get; init; }

    /// <summary>
    /// Gets the operational journey number.
    /// </summary>
    [JsonPropertyName("operationalJourneyNumber")]
    public string OperationalJourneyNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the replacement transport.
    /// </summary>
    [JsonPropertyName("replacementTransport")]
    public ReplacementTransportContract? ReplacementTransport { get; init; }

    /// <summary>
    /// Gets the type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the via stops.
    /// </summary>
    [JsonPropertyName("via")]
    public List<StopPlaceRefViaContract>? Via { get; init; }
}
