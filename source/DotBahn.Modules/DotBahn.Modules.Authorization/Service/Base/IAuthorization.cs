namespace DotBahn.Modules.Authorization.Service.Base;

/// <summary>
/// Interface for the authorization system
/// </summary>
public interface IAuthorization {
    /// <summary>
    /// Authorizes the provided request with the configured credentials.
    /// </summary>
    /// <param name="request">The HTTP request to authorize.</param>
    /// <returns>An authorized request.</returns>
    void AuthorizeRequest(HttpRequestMessage request);
}