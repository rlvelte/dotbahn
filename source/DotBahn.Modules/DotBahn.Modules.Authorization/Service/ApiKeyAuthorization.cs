using DotBahn.Modules.Authorization.Service.Base;

namespace DotBahn.Modules.Authorization.Service;

/// <summary>
/// Service for API Key-based authorization using headers.
/// </summary>
public class ApiKeyAuthorization(AuthorizationOptions configuration) : IAuthorization {
    /// <inheritdoc />
    public void AuthorizeRequest(HttpRequestMessage request) {
        ArgumentNullException.ThrowIfNull(request);
        request.Headers.Add(configuration.HeaderNameClientId, configuration.ClientId);
        request.Headers.Add(configuration.HeaderNameApiKey, configuration.ApiKey);
    }
}
