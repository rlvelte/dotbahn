using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;

namespace DotBahn.Boards;

/// <summary>
/// Client for accessing the Deutsche Bahn OpenBoards API.
/// </summary>
public interface IBoardsClient {
    /// <summary>
    /// Gets an arrival board for the specified query.
    /// </summary>
    /// <param name="query">The board query.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The arrival board.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API responds with a non-success status code.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<Board<ArrivalEntry>> GetArrivalsAsync(BoardQuery query, CancellationToken ct = default);

    /// <summary>
    /// Gets a departure board for the specified query.
    /// </summary>
    /// <param name="query">The board query.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The departure board.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API responds with a non-success status code.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<Board<DepartureEntry>> GetDeparturesAsync(BoardQuery query, CancellationToken ct = default);
}
