using DotBahn.Modules.Auth.Service.Base;

namespace DotBahn.Modules.Auth.Service.Base;

/// <summary>
/// No-op implementation of the authorization system.
/// </summary>
public class NullAuthorizationProvider : IAuthorizationProvider {
    /// <inheritdoc />
    public Task<string> GetAccessTokenAsync() {
        return Task.FromResult(string.Empty);
    }
}
