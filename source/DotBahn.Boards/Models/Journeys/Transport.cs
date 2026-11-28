using DotBahn.Boards.Models.Operators;

namespace DotBahn.Boards.Models.Journeys;

/// <summary>
/// Transport information for a journey event.
/// </summary>
public class Transport {
    /// <summary>
    /// External transport category, e.g. ICE, RE, RB, S.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Internal category before output control; not intended for customer display.
    /// </summary>
    public required string InternalCategory { get; init; }

    /// <summary>
    /// Transport type, e.g. HIGH_SPEED_TRAIN, REGIONAL_TRAIN, CITY_TRAIN.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Transport number.
    /// </summary>
    public required string Number { get; init; }

    /// <summary>
    /// Operational journey number.
    /// </summary>
    public required string OperationalNumber { get; init; }

    /// <summary>
    /// Customer-facing journey description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Marketing or product label, e.g. Sprinter.
    /// </summary>
    public string? Label { get; init; }

    /// <summary>
    /// Deutschlandweite Fahrt ID.
    /// </summary>
    public string? Dfid { get; init; }

    /// <summary>
    /// Deutschlandweite Teil-Linien ID.
    /// </summary>
    public string? Dtid { get; init; }

    /// <summary>
    /// Line information.
    /// </summary>
    public Line? Line { get; init; }

    /// <summary>
    /// Direction texts for the transport.
    /// </summary>
    public IReadOnlyList<string> Direction { get; init; } = [];

    /// <summary>
    /// Stop places along the direction of the transport.
    /// </summary>
    public IReadOnlyList<StopPlace> DirectionStopPlaces { get; init; } = [];

    /// <summary>
    /// Via stops for the transport, including visit-specific details.
    /// </summary>
    public IReadOnlyList<Visit> Via { get; init; } = [];

    /// <summary>
    /// Real type of the replacement transport, if this is a replacement service, e.g. BUS.
    /// </summary>
    public string? ReplacementTransport { get; init; }

    /// <summary>
    /// Administration and operator of the transport.
    /// </summary>
    public required Administration Administration { get; init; }
}
