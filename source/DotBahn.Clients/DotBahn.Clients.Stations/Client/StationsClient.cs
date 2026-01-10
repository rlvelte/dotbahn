using System.ComponentModel;
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
    [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        var response = await GetAsync("/stations", parser, "application/json", query.ToQueryParameters());
        response.Stations.Sort((first, second) => first.Category.CompareTo(second.Category));
        return response;
    }
}