using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for an arrival-board request.
/// </summary>
[ExcludeFromCodeCoverage]
internal record OpenArrivalBoardRequestContract {
    /// <summary>
    /// Gets a value indicating whether line colours are included.
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("includeLineColours")]
    public bool IncludeLineColours { get; init; }

    /// <summary>
    /// Gets a value indicating whether platform accessibility is included.
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("includePlatformAccessibility")]
    public bool IncludePlatformAccessibility { get; init; }

    /// <summary>
    /// Gets the languages.
    /// </summary>
    [JsonPropertyName("languages")]
    public List<string>? Languages { get; init; }

    /// <summary>
    /// Gets the maximum entries per transport type.
    /// </summary>
    [JsonPropertyName("maxEntriesPerTransportType")]
    public int MaxEntriesPerTransportType { get; init; } = 25;

    /// <summary>
    /// Gets the maximum via stops.
    /// </summary>
    [JsonPropertyName("maxViaStops")]
    public int MaxViaStops { get; init; } = 3;

    /// <summary>
    /// Gets a value indicating whether cancellations are excluded.
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("noCancellations")]
    public bool NoCancellations { get; init; }

    /// <summary>
    /// Gets a value indicating whether only replacement transports are included.
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("onlyReplacementTransports")]
    public bool OnlyReplacementTransports { get; init; }

    /// <summary>
    /// Gets the sort by.
    /// </summary>
    [JsonPropertyName("sortBy")]
    public string? SortBy { get; init; }

    /// <summary>
    /// Gets the stop-place or station.
    /// </summary>
    [JsonPropertyName("stopPlaceOrStation")]
    public StopPlaceOrStationRequestContract StopPlaceOrStation { get; init; } = new();

    /// <summary>
    /// Gets the time window.
    /// </summary>
    [JsonPropertyName("timeWindow")]
    public TimeWindowContract? TimeWindow { get; init; }

    /// <summary>
    /// Gets the transport types.
    /// </summary>
    [JsonPropertyName("transportTypes")]
    public List<string>? TransportTypes { get; init; }
}
