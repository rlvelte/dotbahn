using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Stations;
using DotBahn.Tests.Shared.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace DotBahn.Tests.Stations.Extensions;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void AddDotBahnStations_Should_RegisterStationClient_When_Called() {
        var (provider, _, _) = Create();
        using (provider) {
            provider.GetRequiredService<IStationClient>().ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task GetStationsAsync_Should_UseDefaultEndpoint_When_NoConfigurationProvided() {
        var (provider, handler, client) = Create();
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, """{"result":[]}""");

            await client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/stations?logicaloperator=AND&offset=0&limit=10000");
        }
    }

    [Theory]
    [InlineData("https://api.example.com/", "https://api.example.com/stations?logicaloperator=AND&offset=0&limit=10000")]
    [InlineData("https://example.org/stada/v2/", "https://example.org/stada/v2/stations?logicaloperator=AND&offset=0&limit=10000")]
    public async Task GetStationsAsync_Should_UseConfiguredEndpoint_When_ConfigurationProvided(string endpoint, string expectedUri) {
        var (provider, handler, client) = Create(new Uri(endpoint));
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, """{"result":[]}""");

            await client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe(expectedUri);
        }
    }

    [Fact]
    public async Task ValidateAsync_Should_RejectRelativeEndpoint_When_ValidationRuns() {
        var services = new ServiceCollection();
        services.AddDotBahnStations(options => options.BaseEndpoint = new Uri("relative", UriKind.Relative));
        await using var provider = services.BuildServiceProvider();
        var validator = provider.GetRequiredService<IAsyncStartupValidator>();

        var exception = await Should.ThrowAsync<OptionsValidationException>(() => validator.ValidateAsync());

        exception.Message.ShouldContain("DotBahn: BaseEndpoint must be an absolute URI.");
    }

    private static (ServiceProvider Provider, FakeHttpHandler Handler, IStationClient Client) Create(Uri? endpoint = null) {
        var handler = new FakeHttpHandler();
        var services = new ServiceCollection();
        services.AddSingleton<IAuthorization>(new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnStations(options => {
            if (endpoint != null) {
                options.BaseEndpoint = endpoint;
            }
        });
        services.AddHttpClient("DotBahn.Stations").ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();
        return (provider, handler, provider.GetRequiredService<IStationClient>());
    }
}
