using DotBahn.Clients.Shared;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;

namespace DotBahn.Clients.FaSta.Client;

/// <summary>
/// API client for fetching station facilities status (FaSta).
/// </summary>
public class FaStaClient(HttpClient http, IAuthorizationProvider authorization, ICacheProvider cache)
    : BaseClient(http, authorization, cache) {
}
