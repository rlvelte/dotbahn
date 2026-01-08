using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Shared.Queries;
using DotBahn.Clients.Stations.Contracts;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.RequestCache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Stations.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn StaDa'-API.
/// </summary>
public class StationsClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache, IParser<List<StationContract>> stationsParser, IParser<StationContract> stationParser)
    : ClientBase(http, authorization, cache) {
    /// <summary>
    /// Searches for stations using various filter criteria.
    /// </summary>
    /// <param name="searchString">Search pattern for station names. Use '*' as wildcard (e.g., "Berlin*", "*Hbf").</param>
    /// <param name="category">Station category (1-7, where 1 is the largest stations).</param>
    /// <param name="federalState">Federal state name (e.g., "Bayern", "Berlin").</param>
    /// <param name="eva">EVA station number.</param>
    /// <param name="ril">RIL100 identifier.</param>
    /// <param name="logicalOperator">Logical operator for combining filters: "AND" (default) or "OR".</param>
    /// <param name="offset">Number of results to skip for pagination (default: 0).</param>
    /// <param name="limit">Maximum number of results to return (default: 50, max: 10000).</param>
    /// <returns>List of stations matching the search criteria.</returns>
    public async Task<List<StationContract>> GetStationsAsync(string? searchString = null, string? category = null, string? federalState = null, string? eva = null, string? ril = null, string? logicalOperator = null, string? offset = null, string? limit = null) {
        var parameters = QueryParameters.Create()
            .Add("searchstring", searchString)
            .Add("category", category)
            .Add("federalstate", federalState)
            .Add("eva", eva)
            .Add("ril", ril)
            .Add("logicaloperator", logicalOperator)
            .Add("offset", offset)
            .Add("limit", limit);

        return await GetAsync("/stations", stationsParser, "application/json", parameters);
    }
    
    /// <summary>
    /// Gets a specific station by its station number.
    /// </summary>
    /// <param name="eva">The station number (EVA).</param>
    /// <returns>The station details.</returns>
    public async Task<StationContract> GetStationByEvaAsync(string eva) =>
        await GetAsync($"/stations/{eva}", stationParser, "application/json");
}