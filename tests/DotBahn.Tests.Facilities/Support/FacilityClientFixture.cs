using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Facilities;
using DotBahn.Tests.Shared.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Tests.Facilities.Support;

internal sealed class FacilityClientFixture : IDisposable {
    private readonly ServiceProvider _provider;

    public FakeHttpHandler Handler { get; } = new();

    public IFacilityClient Client { get; }

    public FacilityClientFixture(IAuthorization? authorization = null, Action<ClientOptions>? configure = null) {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddSingleton(authorization ?? new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnFacilities(options => {
            options.BaseEndpoint = new Uri("https://api.example.com/");
            configure?.Invoke(options);
        });
        services.AddHttpClient("DotBahn.Facilities").ConfigurePrimaryHttpMessageHandler(() => Handler);

        _provider = services.BuildServiceProvider();
        Client = _provider.GetRequiredService<IFacilityClient>();
    }

    public void Dispose() => _provider.Dispose();
}
