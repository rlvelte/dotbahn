using System.Net;
using DotBahn.Boards;
using DotBahn.Common.Auth;
using DotBahn.Tests.Shared.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace DotBahn.Tests.Boards.Extensions;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void AddDotBahnBoards_Should_RegisterBoardsClient_When_Called() {
        var (provider, _, _) = Create();
        using (provider) {
            provider.GetRequiredService<IBoardsClient>().ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_UseDefaultEndpoint_When_NoConfigurationProvided() {
        var (provider, handler, client) = Create();
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");

            await client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://apis.deutschebahn.com/db-api-marketplace/apis/open-boards/v1/arrivals");
        }
    }

    [Theory]
    [InlineData("https://api.example.com/", "https://api.example.com/departures")]
    [InlineData("https://example.org/boards/v1/", "https://example.org/boards/v1/departures")]
    public async Task GetDeparturesAsync_Should_UseConfiguredEndpoint_When_ConfigurationProvided(string endpoint, string expectedUri) {
        var (provider, handler, client) = Create(new Uri(endpoint));
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");

            await client.GetDeparturesAsync(new BoardQuery(), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe(expectedUri);
        }
    }

    [Fact]
    public async Task ValidateAsync_Should_RejectRelativeEndpoint_When_ValidationRuns() {
        var services = new ServiceCollection();
        services.AddDotBahnBoards(options => options.BaseEndpoint = new Uri("relative", UriKind.Relative));
        await using var provider = services.BuildServiceProvider();
        var validator = provider.GetRequiredService<IAsyncStartupValidator>();

        var exception = await Should.ThrowAsync<OptionsValidationException>(() => validator.ValidateAsync());

        exception.Message.ShouldContain("DotBahn: BaseEndpoint must be an absolute URI.");
    }

    private static (ServiceProvider Provider, FakeHttpHandler Handler, IBoardsClient Client) Create(Uri? endpoint = null) {
        var handler = new FakeHttpHandler();
        var services = new ServiceCollection();
        services.AddSingleton<IAuthorization>(new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnBoards(options => {
            if (endpoint != null) {
                options.BaseEndpoint = endpoint;
            }
        });
        services.AddHttpClient("DotBahn.Boards").ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();
        return (provider, handler, provider.GetRequiredService<IBoardsClient>());
    }
}
