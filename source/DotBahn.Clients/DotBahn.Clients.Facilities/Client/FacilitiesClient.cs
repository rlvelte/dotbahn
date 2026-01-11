using System.ComponentModel;
using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Facilities.Models;
using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Shared.Models;
using DotBahn.Clients.Shared.Options;
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
    private readonly IParser<List<FacilityContract>> _parser;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn FaSta'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    public FacilitiesClient(HttpClient http, IAuthorization authorization, IParser<List<FacilityContract>> parser, ICache? cache = null) 
        : base(http, authorization, cache) {
        _parser = parser;
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
    }

    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="type">Type of facility (e.g., "ESCALATOR", "ELEVATOR").</param>
    /// <param name="state">State of facility (e.g., "ACTIVE", "INACTIVE").</param>
    /// <param name="equipmentNumbers">Array of equipment numbers to filter by.</param>
    /// <param name="stationId">The station id.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public async Task<List<FacilityContract>> GetFacilitiesAsync(string? type = null, string? state = null, string[]? equipmentNumbers = null, string? stationId = null) {
        var queryParams = QueryParameters.Create()
            .Add("type", type)
            .Add("state", state)
            .Add("equipmentnumbers", equipmentNumbers)
            .Add("stationnumber", stationId);

        return await GetAsync("/facilities", _parser, "application/json", queryParams);
    }

    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<List<FacilityContract>> GetFacilitiesAsync(FacilitiesQuery query) {
        return await GetAsync("/facilities", _parser, "application/json", query.ToQueryParameters());
    }
}