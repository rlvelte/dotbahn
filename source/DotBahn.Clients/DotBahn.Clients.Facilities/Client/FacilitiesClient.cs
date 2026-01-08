using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Shared.Queries;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.RequestCache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Facilities.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn FaSta'-API.
/// </summary>
public class FacilitiesClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache, IParser<StationContract> stationParser, IParser<FacilityContract> facilityParser, IParser<List<FacilityContract>> facilitiesParser)
    : ClientBase(http, authorization, cache) {
    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="type">Type of facility (e.g., "ESCALATOR", "ELEVATOR").</param>
    /// <param name="state">State of facility (e.g., "ACTIVE", "INACTIVE").</param>
    /// <param name="equipmentNumbers">Array of equipment numbers to filter by.</param>
    /// <param name="eva">The station number (EVA).</param>
    /// <returns>List of facilities matching the criteria.</returns>
    public async Task<List<FacilityContract>> GetFacilitiesAsync(string? type = null, string? state = null, string[]? equipmentNumbers = null, string? eva = null) {
        var queryParams = QueryParameters.Create()
            .Add("type", type)
            .Add("state", state)
            .Add("equipmentnumbers", equipmentNumbers)
            .Add("stationnumber", eva);

        return await GetAsync("/facilities", facilitiesParser, "application/json", queryParams);
    }

    /// <summary>
    /// Finds a station by its station number.
    /// </summary>
    /// <param name="eva">The station number (EVA).</param>
    /// <returns>The station details.</returns>
    public async Task<StationContract> GetStationByEvaAsync(int eva) =>
        await GetAsync($"/stations/{eva}", stationParser, "application/json");
    
    /// <summary>
    /// Gets a specific facility by its equipment number.
    /// </summary>
    /// <param name="equipmentNumber">The equipment number of the facility.</param>
    /// <returns>The facility details.</returns>
    public async Task<FacilityContract> GetFacilitiesByEquipmentNumberAsync(int equipmentNumber) => 
        await GetAsync($"/facilities/{equipmentNumber}", facilityParser, "application/json");
}