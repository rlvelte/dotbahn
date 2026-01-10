using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Shared.Models;
using DotBahn.Clients.Stations.Contracts;
using DotBahn.Clients.Stations.Models;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Stations.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn StaDa'-API.
/// </summary>
public class StationsClient(HttpClient http, IAuthorization authorization, ICache cache, IParser<StationsResponseContract> parser)
    : ClientBase(http, authorization, cache) {
    /// <summary>
    /// Searches for stations using various filter criteria.
    /// </summary>
    /// <param name="searchString">Search pattern for station names. Use '*' as a wildcard (e.g., "Berlin*", "*Hbf").</param>
    /// <param name="category">Station category (1-7, where 1 is the largest stations).</param>
    /// <param name="federalState">Federal state name (e.g., "Bayern", "Berlin").</param>
    /// <param name="eva">EVA station number.</param>
    /// <param name="ril">RIL100 identifier.</param>
    /// <param name="logicalOperator">Logical operator for combining filters: "AND" (default) or "OR".</param>
    /// <param name="offset">Number of results to skip for pagination (default: 0).</param>
    /// <param name="limit">Maximum number of results to return (default: 50, max: 10000).</param>
    /// <returns>List of stations matching the search criteria.</returns>
    public async Task<StationsResponseContract> GetStationsAsync(string? searchString = null, string? category = null, string? federalState = null, string? eva = null, string? ril = null, string? logicalOperator = null, int? offset = null, int? limit = null) {
        var parameters = QueryParameters.Create()
            .Add("searchstring", searchString)
            .Add("category", category)
            .Add("federalstate", federalState)
            .Add("eva", eva)
            .Add("ril", ril)
            .Add("logicaloperator", logicalOperator)
            .Add("offset", offset.ToString())
            .Add("limit", limit.ToString());

        return await GetAsync("/stations", parser, "application/json", parameters);
    }

    /// <summary>
    /// Searches for stations using a query structure.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <returns>List of stations matching the search criteria.</returns>
    public async Task<StationsResponseContract> GetStationsAsync(StationsQuery query) {
        return await GetAsync("/stations", parser, "application/json", query.ToQueryParameters());
    }

    /// <summary>
    /// Retrieves stations whose names match the given search string.
    /// </summary>
    /// <remarks>If no wildcard is provided or exact is false (default), a trailing '*' is automatically appended to avoid overly strict matches.</remarks>
    /// <param name="name">The station name fragment to search for. Wildcards are supported (e.g., "Berlin*", "*Hbf").</param>
    /// <param name="limit">The limit of returning stations.</param>
    /// <param name="exact">Try to resolve via an exact match of parameter. Default is <c>false</c>.</param>
    /// <returns>A list of stations whose names match the provided search string.</returns>
    public async Task<StationsResponseContract> GetStationsLikeAsync(string name, int limit = 5, bool exact = false) {
        var search = exact || name.Contains('*') ? name : $"{name}*";
        var parameters = QueryParameters.Create().Add("searchstring", search).Add("limit", limit.ToString());
        
        return await GetAsync("/stations", parser, "application/json", parameters);
    }
    
    /// <summary>
    /// Retrieves a single station by its EVA number.
    /// </summary>
    /// <param name="eva">The EVA station number used as a unique identifier.</param>
    /// <returns>The station matching the specified EVA number.</returns>
    public async Task<StationContract> GetStationByEvaAsync(int eva) {
        var parameters = QueryParameters.Create().Add("eva", eva.ToString()).Add("limit", "1");

        var result = await GetAsync("/stations", parser, "application/json", parameters);
        return result.Stations.First();
    }
    
    /// <summary>
    /// Retrieves a single station by its RIL100 identifier.
    /// </summary>
    /// <param name="ril">The RIL100 used as a unique identifier.</param>
    /// <returns>The station matching the specified RIL100 number.</returns>
    public async Task<StationContract> GetStationByRilAsync(string ril) {
        var parameters = QueryParameters.Create().Add("ril", ril).Add("limit", "1");
        
        var result = await GetAsync("/stations", parser, "application/json", parameters);
        return result.Stations.First();
    }
}