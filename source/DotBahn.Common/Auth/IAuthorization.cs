namespace DotBahn.Common.Auth;

/// <summary>
/// Provides authorization for API requests.
/// </summary>
public interface IAuthorization {
    /// <summary>
    /// Authorizes the provided request.
    /// </summary>
    /// <param name="request">The request to add authorization to.</param>
    void AuthorizeRequest(HttpRequestMessage request);
}
