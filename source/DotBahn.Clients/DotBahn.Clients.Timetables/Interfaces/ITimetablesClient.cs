using DotBahn.Data.Timetables.Models;

namespace DotBahn.Clients.Timetables.Interfaces;

/// <summary>
/// Client for accessing the 'Deutsche Bahn Timetables'-API.
/// </summary>
public interface ITimetablesClient {
    /// <summary>
    /// Gets full changes for a specific station.
    /// If a <see cref="Timetable"/> is supplied, the changes are merged onto this instance.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="current">Current timetable on which these changes should apply.</param>
    /// <param name="cancellation">Token to cancel the request.</param>
    /// <returns>A <see cref="Timetable"/> with full change information.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    Task<Timetable> GetFullChangesAsync(int eva, Timetable? current = null, CancellationToken cancellation = default);

    /// <summary>
    /// Gets recent changes for a specific station.
    /// If a <see cref="Timetable"/> is supplied, the changes are merged onto this instance.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="current">Current timetable on which these changes should apply.</param>
    /// <param name="cancellation">Token to cancel the request.</param>
    /// <returns>A <see cref="Timetable"/> with recent change information.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    Task<Timetable> GetRecentChangesAsync(int eva, Timetable? current = null, CancellationToken cancellation = default);

    /// <summary>
    /// Gets the timetable for a specific station and time.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="dateTime">The date and hour (only YYMMDD/HH are used).</param>
    /// <param name="cancellation">Token to cancel the request.</param>
    /// <returns>A <see cref="Timetable"/> for the specified hour.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    Task<Timetable> GetTimetableAsync(int eva, DateTime dateTime, CancellationToken cancellation = default);
}
