using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Timetables.Client;

/// <summary>
/// API client for fetching timetable information.
/// </summary>
public class TimetablesClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache, IParser<TimetableResponseContract> timetableParser)
    : BaseClient(http, authorization, cache) {
    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="TimetableResponseContract"/> with current information.</returns>
    public async Task<TimetableResponseContract> GetFullChangesAsync(string eva) => 
        await GetAsync($"/fchg/{eva}", timetableParser, "application/xml");

    /// <summary>
    /// Gets recent changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="TimetableResponseContract"/> with recent changes.</returns>
    public async Task<TimetableResponseContract> GetRecentChangesAsync(string eva) => 
        await GetAsync($"/rchg/{eva}", timetableParser, "application/xml");

    /// <summary>
    /// Gets the planned timetable for a specific station and time.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="dateTime">The date and hour (only YYMMDD/HH are used).</param>
    /// <returns>A <see cref="TimetableResponseContract"/> for the specified hour.</returns>
    public async Task<TimetableResponseContract> GetPlannedTimetableAsync(string eva, DateTime dateTime) {
        var dateStr = dateTime.ToString("yyMMdd");
        var hourStr = dateTime.ToString("HH");
        return await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", timetableParser, "application/xml");
    }
}
