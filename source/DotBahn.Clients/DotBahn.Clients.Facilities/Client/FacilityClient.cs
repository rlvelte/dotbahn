using DotBahn.Clients.Shared.Base;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;

namespace DotBahn.Clients.Facilities.Client;

/// <summary>
/// API client for fetching station facilities status (FaSta).
/// </summary>
public class FacilityClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache)
    : BaseClient(http, authorization, cache) {
}
