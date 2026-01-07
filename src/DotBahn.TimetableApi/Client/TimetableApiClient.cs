using DotBahn.Cache;
using DotBahn.Cache.Base;
using DotBahn.Core.Client;
using DotBahn.Core.Token.Base;
using DotBahn.Parsing;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Models;
using DotBahn.TimetableApi.Transformers;

namespace DotBahn.TimetableApi.Client;

/// <summary>
/// API client for fetching timetable information.
/// </summary>
public class TimetableApiClient(BaseClientConfiguration configuration, ITokenService tokenService, HttpClient httpClient, ICache? cache = null) 
    : BaseClient(configuration, tokenService, httpClient, cache) {
    
    private readonly XmlParser<TimetableResponseContract> _parser = new();
    private readonly XmlParser<StationsResponseContract> _stationsParser = new();
    
    private readonly TimetableTransformer _transformer = new(
        new StopTransformer(new EventTransformer(), new MessageTransformer())
    );
    private readonly StationTransformer _stationTransformer = new();

    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with current information.</returns>
    public async Task<Timetable> GetFullChangesAsync(string eva) {
        var rawXml = await GetRawDataAsync($"/fchg/{eva}");
        var contract = _parser.Parse(rawXml);
        return _transformer.Transform(contract);
    }

    /// <summary>
    /// Gets recent changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Timetable"/> with recent changes.</returns>
    public async Task<Timetable> GetRecentChangesAsync(string eva) {
        var rawXml = await GetRawDataAsync($"/rchg/{eva}");
        var contract = _parser.Parse(rawXml);
        return _transformer.Transform(contract);
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
        var rawXml = await GetRawDataAsync($"/plan/{eva}/{dateStr}/{hourStr}");
        var contract = _parser.Parse(rawXml);
        return _transformer.Transform(contract);
    }

    /// <summary>
    /// Searches for stations matching the given pattern.
    /// </summary>
    /// <param name="pattern">The search pattern (e.g., station name).</param>
    /// <returns>A list of matching <see cref="StationInfo"/>.</returns>
    public async Task<List<StationInfo>> SearchStationsAsync(string pattern) {
        var rawXml = await GetRawDataAsync($"/station/{pattern}");
        var response = _stationsParser.Parse(rawXml);
        return response.Stations.Select(_stationTransformer.Transform).ToList();
    }
}
