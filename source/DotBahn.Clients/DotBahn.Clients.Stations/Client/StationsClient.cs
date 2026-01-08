using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Stations.Contracts;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.RequestCache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Stations.Client;

/// <summary>
/// Client for fetching station data (StaDa) from Deutsche Bahn.
/// </summary>
public class StationsClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache, IParser<List<StationContract>> stationsParser, IParser<StationContract> stationParser)
    : BaseClient(http, authorization, cache) {
    /// <summary>
    /// Searches for stations using various filter criteria.
    /// </summary>
    /// <param name="searchstring">Search pattern for station names. Use '*' as wildcard (e.g., "Berlin*", "*Hbf").</param>
    /// <param name="category">Station category (1-7, where 1 is the largest stations).</param>
    /// <param name="federalstate">Federal state name (e.g., "Bayern", "Berlin").</param>
    /// <param name="eva">EVA station number.</param>
    /// <param name="ril">RIL100 identifier.</param>
    /// <param name="logicaloperator">Logical operator for combining filters: "AND" (default) or "OR".</param>
    /// <param name="offset">Number of results to skip for pagination (default: 0).</param>
    /// <param name="limit">Maximum number of results to return (default: 50, max: 10000).</param>
    /// <returns>List of stations matching the search criteria.</returns>
    public async Task<List<StationContract>> SearchStationsAsync(string? searchstring = null, int? category = null, string? federalstate = null, int? eva = null, string? ril = null, string? logicaloperator = null, int? offset = null, int? limit = null) {
        var queryParams = QueryParameters.Create()
                                         .Add("searchstring", searchstring)
                                         .Add("category", category)
                                         .Add("federalstate", federalstate)
                                         .Add("eva", eva)
                                         .Add("ril", ril)
                                         .Add("logicaloperator", logicaloperator)
                                         .Add("offset", offset)
                                         .Add("limit", limit);

        return await GetAsync("/stations", stationsParser, "application/json", queryParams);
    }
    
    /// <summary>
    /// Gets a specific station by its station number.
    /// </summary>
    /// <param name="eva">The station number (EVA).</param>
    /// <returns>The station details.</returns>
    public async Task<StationContract> GetStationByEvaAsync(int eva) =>
        await GetAsync($"/stations/{eva}", stationParser, "application/json");
}