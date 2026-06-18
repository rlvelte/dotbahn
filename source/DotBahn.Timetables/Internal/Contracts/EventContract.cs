namespace DotBahn.Timetables.Internal.Contracts;

/// <summary>
/// Raw event information (Arrival/Departure)
/// </summary>
internal record EventContract {
    /// <summary>
    /// Planned time (YYMMDDhhmm format)
    /// </summary>
    public string? PlannedTime { get; init; }

    /// <summary>
    /// Planned platform
    /// </summary>
    public string? PlannedPlatform { get; init; }

    /// <summary>
    /// Planned status (e.g., "p" for planned)
    /// </summary>
    public string? PlannedStatus { get; init; }

    /// <summary>
    /// Changed time (YYMMDDhhmm format)
    /// </summary>
    public string? ChangedTime { get; init; }

    /// <summary>
    /// Changed platform
    /// </summary>
    public string? ChangedPlatform { get; init; }

    /// <summary>
    /// Changed status (e.g., "c" for canceled)
    /// </summary>
    public string? ChangedStatus { get; init; }

    /// <summary>
    /// Hidden (0 or 1)
    /// </summary>
    public string? IsHidden { get; init; }

    /// <summary>
    /// Line
    /// </summary>
    public string? Line { get; init; }

    /// <summary>
    /// Planned path (pipe-separated station list)
    /// </summary>
    public string? PlannedPath { get; init; }

    /// <summary>
    /// Changed path (pipe-separated station list)
    /// </summary>
    public string? ChangedPath { get; init; }

    /// <summary>
    /// Wing train information
    /// </summary>
    public string? Wings { get; init; }

    /// <summary>
    /// Transition information
    /// </summary>
    public string? Transition { get; init; }

    /// <summary>
    /// Planned distant endpoint
    /// </summary>
    public string? PlannedDistantEndpoint { get; init; }

    /// <summary>
    /// Changed distant endpoint
    /// </summary>
    public string? ChangedDistantEndpoint { get; init; }
}
