using DotBahn.Clients.Shared;
using DotBahn.Clients.Shared.Base;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;

namespace DotBahn.Clients.StaDa.Client;

/// <summary>
/// API client for fetching station data (StaDa).
/// </summary>
public class StaDaClient(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache)
    : BaseClient(http, authorization, cache) {
}
