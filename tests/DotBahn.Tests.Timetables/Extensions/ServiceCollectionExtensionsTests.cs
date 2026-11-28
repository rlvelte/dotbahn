using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Tests.Shared.Http;
using DotBahn.Timetables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace DotBahn.Tests.Timetables.Extensions;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void AddDotBahnTimetables_Should_RegisterTimetableClient_When_Called() {
        var (provider, _, _) = Create();
        using (provider) {
            provider.GetRequiredService<ITimetableClient>().ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task GetTimetableAsync_Should_UseDefaultEndpoint_When_NoConfigurationProvided() {
        var (provider, handler, client) = Create();
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, "<timetable/>", "application/xml");

            await client.GetTimetableAsync(8002549, new DateTime(2024, 12, 31, 12, 0, 0), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/plan/8002549/241231/12");
        }
    }

    [Theory]
    [InlineData("https://api.example.com/", "https://api.example.com/plan/1/240101/00")]
    [InlineData("https://example.org/timetables/v1/", "https://example.org/timetables/v1/plan/1/240101/00")]
    public async Task GetTimetableAsync_Should_UseConfiguredEndpoint_When_ConfigurationProvided(string endpoint, string expectedUri) {
        var (provider, handler, client) = Create(new Uri(endpoint));
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, "<timetable/>", "application/xml");

            await client.GetTimetableAsync(1, new DateTime(2024, 1, 1, 0, 0, 0), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe(expectedUri);
        }
    }

    [Fact]
    public async Task ValidateAsync_Should_RejectRelativeEndpoint_When_ValidationRuns() {
        var services = new ServiceCollection();
        services.AddDotBahnTimetables(options => options.BaseEndpoint = new Uri("relative", UriKind.Relative));
        await using var provider = services.BuildServiceProvider();
        var validator = provider.GetRequiredService<IAsyncStartupValidator>();

        var exception = await Should.ThrowAsync<OptionsValidationException>(() => validator.ValidateAsync());

        exception.Message.ShouldContain("DotBahn: BaseEndpoint must be an absolute URI.");
    }

    private static (ServiceProvider Provider, FakeHttpHandler Handler, ITimetableClient Client) Create(Uri? endpoint = null) {
        var handler = new FakeHttpHandler();
        var services = new ServiceCollection();
        services.AddSingleton<IAuthorization>(new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnTimetables(options => {
            if (endpoint != null) {
                options.BaseEndpoint = endpoint;
            }
        });
        services.AddHttpClient("DotBahn.Timetables").ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();
        return (provider, handler, provider.GetRequiredService<ITimetableClient>());
    }
}
