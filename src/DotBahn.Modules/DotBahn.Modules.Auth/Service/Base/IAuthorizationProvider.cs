namespace DotBahn.Modules.Auth.Service.Base;

public interface IAuthorizationProvider {
    Task<string> GetAccessTokenAsync();
}