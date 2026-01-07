namespace DotBahn.Modules.Auth.Base;

public interface IAuthorizationProvider {
    Task<string> GetAccessTokenAsync();
}