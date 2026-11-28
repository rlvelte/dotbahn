using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Stations;
using DotBahn.Tests.Shared.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Tests.Stations.Support;

internal sealed class StationClientFixture : IDisposable {
    private readonly ServiceProvider _provider;

    public FakeHttpHandler Handler { get; } = new();

    public IStationClient Client { get; }

    public StationClientFixture(IAuthorization? authorization = null, Action<ClientOptions>? configure = null) {
        var services = new ServiceCollection();
        services.AddSingleton(authorization ?? new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnStations(options => {
            options.BaseEndpoint = new Uri("https://api.example.com/");
            configure?.Invoke(options);
        });
        services.AddHttpClient("DotBahn.Stations").ConfigurePrimaryHttpMessageHandler(() => Handler);

        _provider = services.BuildServiceProvider();
        Client = _provider.GetRequiredService<IStationClient>();
    }

    public void Dispose() => _provider.Dispose();
}
