using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Shared.Base;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.RequestCache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Facilities.Client;

/// <summary>
/// Client for fetching station facilities status (FaSta).
/// </summary>
public class FacilitiesClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache, IParser<StationContract> stationParser, IParser<FacilityContract> facilityParser, IParser<List<FacilityContract>> facilitiesParser)
    : BaseClient(http, authorization, cache) {
    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="type">Type of facility (e.g., "ESCALATOR", "ELEVATOR").</param>
    /// <param name="state">State of facility (e.g., "ACTIVE", "INACTIVE").</param>
    /// <param name="equipmentnumbers">Array of equipment numbers to filter by.</param>
    /// <param name="stationnumbers">Array of station numbers to filter by.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    public async Task<List<FacilityContract>> FindFacilitiesAsync(string? type = null, string? state = null, int[]? equipmentnumbers = null, int[]? stationnumbers = null) {
        var queryParams = QueryParameters.Create()
                                         .Add("type", type)
                                         .Add("state", state)
                                         .Add("equipmentnumbers", equipmentnumbers)
                                         .Add("stationnumbers", stationnumbers);
        
        return await GetAsync("/facilities", facilitiesParser, "application/json", queryParams);
    }
    
    /// <summary>
    /// Gets a specific facility by its equipment number.
    /// </summary>
    /// <param name="equipmentNumber">The equipment number of the facility.</param>
    /// <returns>The facility details.</returns>
    private async Task<FacilityContract> GetFacilitiesByEquipmentNumberAsync(int equipmentNumber) => 
        await GetAsync($"/facilities/{equipmentNumber}", facilityParser, "application/json");
    
    /// <summary>
    /// Finds a station by its station number.
    /// </summary>
    /// <param name="eva">The station number (EVA).</param>
    /// <returns>The station details.</returns>
    public async Task<StationContract> FindStationByEvaAsync(int eva) => 
        await GetAsync($"/stations/{eva}", stationParser, "application/json");
}