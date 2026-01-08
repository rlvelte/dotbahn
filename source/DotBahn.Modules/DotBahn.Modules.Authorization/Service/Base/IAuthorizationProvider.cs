namespace DotBahn.Modules.Authorization.Service.Base;

public interface IAuthorizationProvider {
    Task AuthorizeRequestAsync(HttpRequestMessage request);
}