using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Models;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Transformer;

namespace DotBahn.Clients.Timetables.Client;

/// <summary>
/// API client for fetching timetable information.
/// </summary>
public class TimetablesClient(HttpClient http, IAuthorizationProvider authorization, ICacheProvider cache, ITransformer<TimetableResponseContract, Timetable> timetableTransformer, ITransformer<StationContract, StationInfo> stationTransformer, IParser<TimetableResponseContract> timetableParser, IParser<StationsResponseContract> stationsParser)
    : BaseClient(http, authorization, cache) {
    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with current information.</returns>
    public async Task<Timetable> GetFullChangesAsync(string eva) {
        var contract = await GetAsync($"/fchg/{eva}", timetableParser);
        return timetableTransformer.Transform(contract);
    }

    /// <summary>
    /// Gets recent changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with recent changes.</returns>
    public async Task<Timetable> GetRecentChangesAsync(string eva) {
        var contract = await GetAsync($"/rchg/{eva}", timetableParser);
        return timetableTransformer.Transform(contract);
    }

    /// <summary>
    /// Gets the planned timetable for a specific station and time.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="dateTime">The date and hour (only YYMMDD/HH are used).</param>
    /// <returns>A <see cref="Timetable"/> for the specified hour.</returns>
    public async Task<Timetable> GetPlannedTimetableAsync(string eva, DateTime dateTime) {
        var dateStr = dateTime.ToString("yyMMdd");
        var hourStr = dateTime.ToString("HH");
        var contract = await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", timetableParser);
        return timetableTransformer.Transform(contract);
    }

    /// <summary>
    /// Searches for stations matching the given pattern.
    /// </summary>
    /// <param name="pattern">The search pattern (e.g., station name).</param>
    /// <returns>A list of matching <see cref="StationInfo"/>.</returns>
    public async Task<List<StationInfo>> SearchStationsAsync(string pattern) {
        var response = await GetAsync($"/station/{pattern}", stationsParser);
        return response.Stations.Select(stationTransformer.Transform).ToList();
    }
}
