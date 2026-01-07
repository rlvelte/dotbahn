using DotBahn.Clients.Shared;
using DotBahn.Modules.Auth.Base;
using DotBahn.Modules.Cache.Base;
using DotBahn.Modules.Parsing;
using DotBahn.Modules.Parsing.Base;
using DotBahn.TimetableApi.Configuration;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Models;
using Microsoft.Extensions.Options;

namespace DotBahn.TimetableApi.Client;

/// <summary>
/// API client for fetching timetable information.
/// </summary>
public class TimetableApiClient(IOptions<TimetableConfiguration> options, IAuthorizationProvider tokenService, HttpClient httpClient, ITransformer<TimetableResponseContract, Timetable> transformer, ITransformer<StationContract, StationInfo> stationTransformer, ICacheProvider? cache = null)
    : BaseClient(tokenService, httpClient, cache) {
    private readonly TimetableConfiguration _options = options.Value;
    
    private readonly XmlParser<TimetableResponseContract> _parser = new();
    private readonly XmlParser<StationsResponseContract> _stationsParser = new();

    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with current information.</returns>
    public async Task<Timetable> GetFullChangesAsync(string eva) {
        var rawXml = await GetRawDataAsync($"/fchg/{eva}", expiration: _options.Cache.DefaultExpiration);
        var contract = _parser.Parse(rawXml);
        return transformer.Transform(contract);
    }

    /// <summary>
    /// Gets recent changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with recent changes.</returns>
    public async Task<Timetable> GetRecentChangesAsync(string eva) {
        var rawXml = await GetRawDataAsync($"/rchg/{eva}", expiration: _options.Cache.DefaultExpiration);
        var contract = _parser.Parse(rawXml);
        return transformer.Transform(contract);
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
        var rawXml = await GetRawDataAsync($"/plan/{eva}/{dateStr}/{hourStr}", expiration: _options.Cache.DefaultExpiration);
        var contract = _parser.Parse(rawXml);
        return transformer.Transform(contract);
    }

    /// <summary>
    /// Searches for stations matching the given pattern.
    /// </summary>
    /// <param name="pattern">The search pattern (e.g., station name).</param>
    /// <returns>A list of matching <see cref="StationInfo"/>.</returns>
    public async Task<List<StationInfo>> SearchStationsAsync(string pattern) {
        var rawXml = await GetRawDataAsync($"/station/{pattern}", expiration: _options.Cache.DefaultExpiration);
        var response = _stationsParser.Parse(rawXml);
        return response.Stations.Select(stationTransformer.Transform).ToList();
    }
}
