namespace DotBahn.Modules.Authorization.Service.Base;

/// <summary>
/// No-op implementation of the authorization system.
/// </summary>
public class NullAuthorizationProvider : IAuthorizationProvider {
    /// <inheritdoc />
    public Task AuthorizeRequestAsync(HttpRequestMessage request) {
        return Task.CompletedTask;
    }
}
