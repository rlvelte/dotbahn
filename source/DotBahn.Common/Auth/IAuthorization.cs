namespace DotBahn.Common.Auth;

/// <summary>
/// Provides authorization for API requests
/// </summary>
public interface IAuthorization {
    /// <summary>
    /// Authorizes the provided request
    /// </summary>
    /// <param name="request">The request to add authorization to</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <c>null</c></exception>
    void AuthorizeRequest(HttpRequestMessage request);
}
