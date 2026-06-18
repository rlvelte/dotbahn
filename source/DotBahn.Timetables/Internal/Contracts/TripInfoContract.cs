namespace DotBahn.Timetables.Internal.Contracts;

/// <summary>
/// Raw trip label (train information)
/// </summary>
internal record TripInfoContract {
    /// <summary>
    /// Filter flags
    /// </summary>
    public string? FilterFlags { get; init; }

    /// <summary>
    /// Trip type (p, e, f)
    /// </summary>
    public string? TripType { get; init; }

    /// <summary>
    /// Train owner
    /// </summary>
    public string? Owner { get; init; }

    /// <summary>
    /// Train category (e.g., ICE)
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Train number
    /// </summary>
    public string? Number { get; init; }
}
