using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Facilities.Query;
using DotBahn.Clients.Facilities.Transformer;
using DotBahn.Clients.Shared.Client;
using DotBahn.Clients.Shared.Options;
using DotBahn.Data.Facilities.Models;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Facilities.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn FaSta'-API.
/// </summary>
public class FacilitiesClient : ClientBase {
    private readonly IParser<IEnumerable<FacilityContract>> _parser;
    private readonly ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>> _transformer;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn FaSta'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    public FacilitiesClient(HttpClient http, IAuthorization authorization, IParser<IEnumerable<FacilityContract>> parser, ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>> transformer, ICache? cache = null) 
        : base(http, authorization, cache) {
        _parser = parser;
        _transformer = transformer;
    }
    
    /// <summary>
    /// Client for accessing 'Deutsche Bahn FaSta'-API.
    /// </summary>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <param name="cache">The cache options for the client.</param>
    public FacilitiesClient(ClientOptions options, AuthorizationOptions auth, CacheOptions? cache = null)
        : base(options, auth, cache) {
        _parser = new JsonParser<List<FacilityContract>>();
        _transformer = new FacilityTransformer();
    }

    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<IEnumerable<Facility>> GetFacilitiesAsync(FacilitiesQuery query) {
        var result = (await GetAsync("/facilities", _parser, "application/json", query.ToQueryParameters())).ToList();
        return _transformer.Transform(result);
    }
}