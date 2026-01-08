using DotBahn.Modules.Authorization.Service.Base;

namespace DotBahn.Modules.Authorization.Service;

/// <summary>
/// Service for API Key-based authorization using headers.
/// </summary>
public class ApiKeyAuthorizationProvider(ModuleOptions configuration) : IAuthorizationProvider {
    /// <inheritdoc />
    public Task AuthorizeRequestAsync(HttpRequestMessage request) {
        request.Headers.Add("DB-Client-Id", configuration.ClientId);
        request.Headers.Add("DB-Api-Key", configuration.ClientSecret);
        return Task.CompletedTask;
    }
}
