using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a journey information
/// </summary>
[ExcludeFromCodeCoverage]
internal record JourneyInfoContract {
    /// <summary>
    /// Gets the destination
    /// </summary>
    [JsonPropertyName("destination")]
    public StopPlaceRefWithCancelContract Destination { get; init; } = new();

    /// <summary>
    /// Gets the differing destination
    /// </summary>
    [JsonPropertyName("differingDestination")]
    public StopPlaceRefContract? DifferingDestination { get; init; }

    /// <summary>
    /// Gets the differing origin
    /// </summary>
    [JsonPropertyName("differingOrigin")]
    public StopPlaceRefContract? DifferingOrigin { get; init; }

    /// <summary>
    /// Gets the header administration identifier
    /// </summary>
    [JsonPropertyName("headerAdministrationId")]
    public string HeaderAdministrationId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the header journey number
    /// </summary>
    [JsonPropertyName("headerJourneyNumber")]
    public int HeaderJourneyNumber { get; init; }

    /// <summary>
    /// Gets a value indicating whether the journey is cancelled
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("journeyCancelled")]
    public bool JourneyCancelled { get; init; }

    /// <summary>
    /// Gets the origin
    /// </summary>
    [JsonPropertyName("origin")]
    public StopPlaceRefWithCancelContract Origin { get; init; } = new();

    /// <summary>
    /// Gets the scheduled start time
    /// </summary>
    [JsonPropertyName("scheduledStartTime")]
    public string ScheduledStartTime { get; init; } = string.Empty;

    /// <summary>
    /// Gets the transport at start
    /// </summary>
    [JsonPropertyName("transportAtStart")]
    public TransportContract TransportAtStart { get; init; } = new();

    /// <summary>
    /// Gets the type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
}
