namespace DotBahn.Modules.Authorization;

/// <summary>
/// No-op implementation of the authorization system.
/// </summary>
public class NullAuthorization : IAuthorization {
    /// <inheritdoc />
    public void AuthorizeRequest(HttpRequestMessage request) { }
}
