using DotBahn.Clients.Shared;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Transformer;
using DotBahn.TimetableApi.Configuration;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Models;
using Microsoft.Extensions.Options;

namespace DotBahn.TimetableApi.Client;

/// <summary>
/// API client for fetching timetable information.
/// </summary>
public class TimetableApiClient(IOptions<TimetableConfiguration> options, IAuthorizationProvider authorizationProvider, HttpClient httpClient, ITransformer<TimetableResponseContract, Timetable> timetableTransformer, ITransformer<StationContract, StationInfo> stationTransformer, ICacheProvider? cacheProvider = null)
    : BaseClient(httpClient, authorizationProvider, cacheProvider) {
    private readonly TimetableConfiguration _options = options.Value;
    
    private readonly XmlParser<TimetableResponseContract> _timetableParser = new();
    private readonly XmlParser<StationsResponseContract> _stationsParser = new();

    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with current information.</returns>
    public async Task<Timetable> GetFullChangesAsync(string eva) {
        var contract = await GetAsync($"/fchg/{eva}", _timetableParser, expiration: _options.Cache.DefaultExpiration);
        return timetableTransformer.Transform(contract);
    }

    /// <summary>
    /// Gets recent changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with recent changes.</returns>
    public async Task<Timetable> GetRecentChangesAsync(string eva) {
        var contract = await GetAsync($"/rchg/{eva}", _timetableParser, expiration: _options.Cache.DefaultExpiration);
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
        var contract = await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", _timetableParser, expiration: _options.Cache.DefaultExpiration);
        return timetableTransformer.Transform(contract);
    }

    /// <summary>
    /// Searches for stations matching the given pattern.
    /// </summary>
    /// <param name="pattern">The search pattern (e.g., station name).</param>
    /// <returns>A list of matching <see cref="StationInfo"/>.</returns>
    public async Task<List<StationInfo>> SearchStationsAsync(string pattern) {
        var response = await GetAsync($"/station/{pattern}", _stationsParser, expiration: _options.Cache.DefaultExpiration);
        return response.Stations.Select(stationTransformer.Transform).ToList();
    }
}
