using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Tests.Shared.Http;
using DotBahn.Timetables;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Tests.Timetables.Support;

internal sealed class TimetableClientFixture : IDisposable {
    private readonly ServiceProvider _provider;

    public FakeHttpHandler Handler { get; } = new();

    public ITimetableClient Client { get; }

    public TimetableClientFixture(IAuthorization? authorization = null, Action<ClientOptions>? configure = null) {
        var services = new ServiceCollection();
        services.AddSingleton(authorization ?? new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnTimetables(options => {
            options.BaseEndpoint = new Uri("https://api.example.com/");
            configure?.Invoke(options);
        });
        services.AddHttpClient("DotBahn.Timetables").ConfigurePrimaryHttpMessageHandler(() => Handler);

        _provider = services.BuildServiceProvider();
        Client = _provider.GetRequiredService<ITimetableClient>();
    }

    public void Dispose() => _provider.Dispose();
}
