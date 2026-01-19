using DotBahn.Clients.Shared.Client;
using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Stations.Contracts;
using DotBahn.Clients.Stations.Transformer;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Stations.Models;
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
    private readonly ITransformer<IEnumerable<Station>, StationsResponseContract> _transformer;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn StaDa'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="transformer">The transformer for this model and contract types.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    public StationsClient(HttpClient http, IAuthorization authorization, IParser<StationsResponseContract> parser, ITransformer<IEnumerable<Station>, StationsResponseContract> transformer, ICache? cache = null) 
        : base(http, authorization, cache) {
        _parser = parser;
        _transformer = transformer;
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
        _transformer = new StationTransformer();
    }

    /// <summary>
    /// Searches for stations using a query structure.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <returns>List of stations matching the search criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<IEnumerable<Station>> GetStationsAsync(Query.StationsQuery query) {
        var response = await GetAsync("/stations", _parser, "application/json", query.ToQueryParameters());
        response.Stations.Sort((first, second) => first.Category.CompareTo(second.Category));
        
        return _transformer.Transform(response);
    }
}