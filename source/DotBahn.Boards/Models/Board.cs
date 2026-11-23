
using DotBahn.Boards.Models.Entries;
using DotBahn.Boards.Models.Messages;

namespace DotBahn.Boards.Models;

/// <summary>
/// A board of arrival or departure events for one or more stop places.
/// </summary>
/// <typeparam name="TEntry">The type of board entry.</typeparam>
public class Board<TEntry> where TEntry : BoardEntry {
    /// <summary>
    /// Stop places the board was created for.
    /// </summary>
    public IReadOnlyList<StopPlace> ConsideredStopPlaces { get; init; } = [];

    /// <summary>
    /// Entries matching the query.
    /// </summary>
    public IReadOnlyList<TEntry> Entries { get; init; } = [];

    /// <summary>
    /// Station-level disruption messages.
    /// </summary>
    public IReadOnlyList<Message> StationMessages { get; init; } = [];

    /// <summary>
    /// Transport types whose results were truncated to the configured maximum per transport type.
    /// </summary>
    public IReadOnlyList<string> TruncatedTransportTypes { get; init; } = [];
}
