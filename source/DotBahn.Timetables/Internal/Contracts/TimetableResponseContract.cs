namespace DotBahn.Timetables.Internal.Contracts;

/// <summary>
/// Raw timetable response
/// </summary>
internal record TimetableResponseContract {
    /// <summary>
    /// Station name or ID
    /// </summary>
    public string Station { get; init; } = string.Empty;

    /// <summary>
    /// List of stops in the timetable
    /// </summary>
    public List<StopDataContract> Stops { get; init; } = [];
}
