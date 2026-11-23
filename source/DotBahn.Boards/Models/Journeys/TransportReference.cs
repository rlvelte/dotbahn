using DotBahn.Boards.Models.Operators;

namespace DotBahn.Boards.Models.Journeys;

/// <summary>
/// Reference to another transport, e.g. for continuation, relief, replacement
/// </summary>
public class TransportReference {
    /// <summary>
    /// Identifier of the referenced journey.
    /// </summary>
    public required string JourneyId { get; init; }

    /// <summary>
    /// Category of the referenced transport.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Internal category of the referenced transport.
    /// </summary>
    public string? InternalCategory { get; init; }

    /// <summary>
    /// Transport type of the referenced transport.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Number of the referenced transport.
    /// </summary>
    public required string Number { get; init; }

    /// <summary>
    /// Operational journey number of the referenced transport.
    /// </summary>
    public string? OperationalNumber { get; init; }

    /// <summary>
    /// Customer-facing journey description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Marketing or product label.
    /// </summary>
    public string? Label { get; init; }

    /// <summary>
    /// Line information.
    /// </summary>
    public Line? Line { get; init; }

    /// <summary>
    /// Deutschlandweite Fahrt ID.
    /// </summary>
    public string? Dfid { get; init; }

    /// <summary>
    /// Deutschlandweite Teil-Linien ID.
    /// </summary>
    public string? Dtid { get; init; }

    /// <summary>
    /// Administration and operator of the referenced transport.
    /// </summary>
    public Administration? Administration { get; init; }

    /// <summary>
    /// Origin of the referenced transport, if applicable.
    /// </summary>
    public Visit? Origin { get; init; }

    /// <summary>
    /// Destination of the referenced transport, if applicable.
    /// </summary>
    public Visit? Destination { get; init; }

    /// <summary>
    /// Differing origin stop place, if applicable.
    /// </summary>
    public StopPlace? DifferingOrigin { get; init; }

    /// <summary>
    /// Differing destination stop place, if applicable.
    /// </summary>
    public StopPlace? DifferingDestination { get; init; }

    /// <summary>
    /// Stop place where the working portion is separated, if applicable.
    /// </summary>
    public StopPlace? SeparationAt { get; init; }
}
