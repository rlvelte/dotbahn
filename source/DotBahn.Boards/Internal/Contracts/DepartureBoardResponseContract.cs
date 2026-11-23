using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a departure-board response
/// </summary>
[ExcludeFromCodeCoverage]
internal record DepartureBoardResponseContract {
    /// <summary>
    /// Gets the administrations
    /// </summary>
    [JsonPropertyName("administrations")]
    public List<AdministrationExtendedMultiLanguageContract>? Administrations { get; init; }

    /// <summary>
    /// Gets the departures
    /// </summary>
    [JsonPropertyName("departures")]
    public List<DepartureContract>? Departures { get; init; }

    /// <summary>
    /// Gets the disruption communications
    /// </summary>
    [JsonPropertyName("disruptionCommunications")]
    public List<int>? DisruptionCommunications { get; init; }

    /// <summary>
    /// Gets the messages
    /// </summary>
    [JsonPropertyName("messages")]
    public MessagesMultiLanguageContract? Messages { get; init; }

    /// <summary>
    /// Gets the meta
    /// </summary>
    [JsonPropertyName("meta")]
    public BoardMetaContract Meta { get; init; } = new();

    /// <summary>
    /// Gets the stations
    /// </summary>
    [JsonPropertyName("stations")]
    public List<StationMultiLanguageContract>? Stations { get; init; }

    /// <summary>
    /// Gets the stop places
    /// </summary>
    [JsonPropertyName("stopPlaces")]
    public List<StopPlaceMultiLanguageContract>? StopPlaces { get; init; }
}
