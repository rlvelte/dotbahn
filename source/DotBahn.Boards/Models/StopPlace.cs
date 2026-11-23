namespace DotBahn.Boards.Models;

/// <summary>
/// A stop place where a journey stops.
/// </summary>
public class StopPlace {
    /// <summary>
    /// EVA number of the stop place.
    /// </summary>
    public required string EvaNumber { get; init; }

    /// <summary>
    /// Name of the stop place.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// STADA identifier of the parent station, if available.
    /// </summary>
    public string? StadaId { get; init; }

    /// <summary>
    /// Name of the parent station, if available.
    /// </summary>
    public string? StationName { get; init; }

    /// <summary>
    /// DHID / IFOPT of the stop place, if available.
    /// </summary>
    public string? Dhid { get; init; }

    /// <summary>
    /// Symbol of the stop place, if available.
    /// </summary>
    public string? Symbol { get; init; }

    /// <summary>
    /// Name of the symbol of the stop place, if available.
    /// </summary>
    public string? SymbolName { get; init; }
}
