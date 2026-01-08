using DotBahn.Clients.Shared.Base;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;

namespace DotBahn.Clients.Stations.Client;

/// <summary>
/// API client for fetching station data (StaDa).
/// </summary>
public class StationClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache)
    : BaseClient(http, authorization, cache) {
}
