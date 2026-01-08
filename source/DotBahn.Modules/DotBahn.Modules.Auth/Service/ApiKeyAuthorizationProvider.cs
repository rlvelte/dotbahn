using DotBahn.Modules.Auth.Options;
using DotBahn.Modules.Auth.Service.Base;

namespace DotBahn.Modules.Auth.Service;

/// <summary>
/// Service for API Key-based authorization using headers.
/// </summary>
public class ApiKeyAuthorizationProvider(AuthOptions configuration) : IAuthorizationProvider {
    /// <inheritdoc />
    public Task AuthorizeRequestAsync(HttpRequestMessage request) {
        request.Headers.Add("DB-Client-Id", configuration.ClientId);
        request.Headers.Add("DB-Api-Key", configuration.ClientSecret);
        return Task.CompletedTask;
    }
}
