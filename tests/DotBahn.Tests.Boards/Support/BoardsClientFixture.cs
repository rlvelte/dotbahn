using DotBahn.Boards;
using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Tests.Shared.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Tests.Boards.Support;

internal sealed class BoardsClientFixture : IDisposable {
    private readonly ServiceProvider _provider;

    public FakeHttpHandler Handler { get; } = new();

    public IBoardsClient Client { get; }

    public BoardsClientFixture(IAuthorization? authorization = null, Action<ClientOptions>? configure = null) {
        var services = new ServiceCollection();
        services.AddSingleton(authorization ?? new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnBoards(options => {
            options.BaseEndpoint = new Uri("https://api.example.com/");
            configure?.Invoke(options);
        });
        services.AddHttpClient("DotBahn.Boards").ConfigurePrimaryHttpMessageHandler(() => Handler);

        _provider = services.BuildServiceProvider();
        Client = _provider.GetRequiredService<IBoardsClient>();
    }

    public void Dispose() => _provider.Dispose();
}
