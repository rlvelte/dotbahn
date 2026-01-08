using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Facilities.Models;
using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Shared.Models;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.RequestCache.Service.Base;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Facilities.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn FaSta'-API.
/// </summary>
public class FacilitiesClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache, IParser<List<FacilityContract>> facilitiesParser)
    : ClientBase(http, authorization, cache) {
    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="type">Type of facility (e.g., "ESCALATOR", "ELEVATOR").</param>
    /// <param name="state">State of facility (e.g., "ACTIVE", "INACTIVE").</param>
    /// <param name="equipmentNumbers">Array of equipment numbers to filter by.</param>
    /// <param name="stationId">The station id.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    public async Task<List<FacilityContract>> GetFacilitiesAsync(string? type = null, string? state = null, string[]? equipmentNumbers = null, string? stationId = null) {
        var queryParams = QueryParameters.Create()
            .Add("type", type)
            .Add("state", state)
            .Add("equipmentnumbers", equipmentNumbers)
            .Add("stationnumber", stationId);

        return await GetAsync("/facilities", facilitiesParser, "application/json", queryParams);
    }

    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    public async Task<List<FacilityContract>> GetFacilitiesAsync(FacilitiesQuery query) {
        return await GetAsync("/facilities", facilitiesParser, "application/json", query.ToQueryParameters());
    }
}