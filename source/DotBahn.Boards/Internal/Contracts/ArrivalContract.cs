using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for an arrival
/// </summary>
[ExcludeFromCodeCoverage]
internal record ArrivalContract {
    /// <summary>
    /// Gets a value indicating whether this is additional
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("additional")]
    public bool Additional { get; init; }

    /// <summary>
    /// Gets the arrival identifier
    /// </summary>
    [JsonPropertyName("arrivalId")]
    public string ArrivalId { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this is cancelled
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("cancelled")]
    public bool Cancelled { get; init; }

    /// <summary>
    /// Gets the codeshares
    /// </summary>
    [JsonPropertyName("codeshares")]
    public List<CodeShareContract>? Codeshares { get; init; }

    /// <summary>
    /// Gets the continuation by
    /// </summary>
    [JsonPropertyName("continuationBy")]
    public List<TransportDestinationRefContract>? ContinuationBy { get; init; }

    /// <summary>
    /// Gets the EVA number
    /// </summary>
    [JsonPropertyName("evaNumber")]
    public string EvaNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the journey identifier
    /// </summary>
    [JsonPropertyName("journeyId")]
    public string JourneyId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the journey information
    /// </summary>
    [JsonPropertyName("journeyInfo")]
    public JourneyInfoContract JourneyInfo { get; init; } = new();

    /// <summary>
    /// Gets the messages
    /// </summary>
    [JsonPropertyName("messages")]
    public List<int>? Messages { get; init; }

    /// <summary>
    /// Gets a value indicating whether there is no passenger change
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("noPassengerChange")]
    public bool NoPassengerChange { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is on demand
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("onDemand")]
    public bool OnDemand { get; init; }

    /// <summary>
    /// Gets a value indicating whether there were past disruptions
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("pastDisruptions")]
    public bool? PastDisruptions { get; init; }

    /// <summary>
    /// Gets the platform
    /// </summary>
    [JsonPropertyName("platform")]
    public PlatformContract? Platform { get; init; }

    /// <summary>
    /// Gets the platform history
    /// </summary>
    [JsonPropertyName("platformHistory")]
    public List<PlatformHistoryContract>? PlatformHistory { get; init; }

    /// <summary>
    /// Gets the platform schedule
    /// </summary>
    [JsonPropertyName("platformSchedule")]
    public PlatformContract? PlatformSchedule { get; init; }

    /// <summary>
    /// Gets the relief by
    /// </summary>
    [JsonPropertyName("reliefBy")]
    public List<TransportOriginRefContract>? ReliefBy { get; init; }

    /// <summary>
    /// Gets the relief for
    /// </summary>
    [JsonPropertyName("reliefFor")]
    public List<TransportOriginRefContract>? ReliefFor { get; init; }

    /// <summary>
    /// Gets the replaced by
    /// </summary>
    [JsonPropertyName("replacedBy")]
    public List<TransportOriginRefContract>? ReplacedBy { get; init; }

    /// <summary>
    /// Gets the replacement for
    /// </summary>
    [JsonPropertyName("replacementFor")]
    public List<TransportOriginRefContract>? ReplacementFor { get; init; }

    /// <summary>
    /// Gets the time
    /// </summary>
    [JsonPropertyName("time")]
    public string Time { get; init; } = string.Empty;

    /// <summary>
    /// Gets the time schedule
    /// </summary>
    [JsonPropertyName("timeSchedule")]
    public string TimeSchedule { get; init; } = string.Empty;

    /// <summary>
    /// Gets the time type
    /// </summary>
    [JsonPropertyName("timeType")]
    public string TimeType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the transport
    /// </summary>
    [JsonPropertyName("transport")]
    public TransportWithDirectionAndViaContract Transport { get; init; } = new();

    /// <summary>
    /// Gets the travels with
    /// </summary>
    [JsonPropertyName("travelsWith")]
    public List<TransportOriginRefContract>? TravelsWith { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle sequence is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("vehicleSequenceAvailable")]
    public bool? VehicleSequenceAvailable { get; init; }
}
