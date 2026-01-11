using System.ComponentModel;
using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Shared.Models;
using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Stations.Contracts;
using DotBahn.Clients.Stations.Models;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Stations.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn StaDa'-API.
/// </summary>
public class StationsClient : ClientBase {
    private readonly IParser<StationsResponseContract> _parser;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn StaDa'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    public StationsClient(HttpClient http, IAuthorization authorization, IParser<StationsResponseContract> parser, ICache? cache = null) 
        : base(http, authorization, cache) {
        _parser = parser;
    }

    /// <summary>
    /// Client for accessing 'Deutsche Bahn StaDa'-API.
    /// </summary>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <param name="cache">The cache options for the client.</param>
    public StationsClient(ClientOptions options, AuthorizationOptions auth, CacheOptions? cache = null)
        : base(options, auth, cache) {
        _parser = new JsonParser<StationsResponseContract>();
    }
    
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
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
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

        return await GetAsync("/stations", _parser, "application/json", parameters);
    }

    /// <summary>
    /// Searches for stations using a query structure.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <returns>List of stations matching the search criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<StationsResponseContract> GetStationsAsync(StationsQuery query) {
        var response = await GetAsync("/stations", _parser, "application/json", query.ToQueryParameters());
        response.Stations.Sort((first, second) => first.Category.CompareTo(second.Category));
        return response;
    }
}