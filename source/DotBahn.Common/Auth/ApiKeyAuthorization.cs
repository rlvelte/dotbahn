namespace DotBahn.Common.Auth;

/// <inheritdoc />
public class ApiKeyAuthorization(AuthorizationOptions configuration) : IAuthorization {
    private const string HeaderNameClientId = "DB-Client-Id";
    private const string HeaderNameApiKey = "DB-Api-Key";

    /// <inheritdoc />
    public void AuthorizeRequest(HttpRequestMessage request) {
        ArgumentNullException.ThrowIfNull(request);

        request.Headers.Add(HeaderNameClientId, configuration.ClientId);
        request.Headers.Add(HeaderNameApiKey, configuration.ApiKey);
    }
}
