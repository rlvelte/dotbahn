using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Timetables.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn Timetables'-API.
/// </summary>
public class TimetablesClient(HttpClient http, IAuthorization authorization, ICache cache, IParser<TimetableResponseContract> timetableParser)
    : ClientBase(http, authorization, cache) {
    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="TimetableResponseContract"/> with current information.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<TimetableResponseContract> GetFullChangesAsync(int eva) =>
        await GetAsync($"/fchg/{eva}", timetableParser, "application/xml");

    /// <summary>
    /// Gets recent changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="TimetableResponseContract"/> with recent changes.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<TimetableResponseContract> GetRecentChangesAsync(int eva) => 
        await GetAsync($"/rchg/{eva}", timetableParser, "application/xml");

    /// <summary>
    /// Gets the planned timetable for a specific station and time.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="dateTime">The date and hour (only YYMMDD/HH are used).</param>
    /// <returns>A <see cref="TimetableResponseContract"/> for the specified hour.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<TimetableResponseContract> GetPlannedTimetableAsync(int eva, DateTime dateTime) {
        var dateStr = dateTime.ToString("yyMMdd");
        var hourStr = dateTime.ToString("HH");
        return await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", timetableParser, "application/xml");
    }
}
