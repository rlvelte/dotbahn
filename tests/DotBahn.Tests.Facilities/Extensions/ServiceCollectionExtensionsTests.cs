using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Facilities;
using DotBahn.Tests.Shared.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace DotBahn.Tests.Facilities.Extensions;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void AddDotBahnFacilities_Should_RegisterFacilityClient_When_Called() {
        var (provider, _, _) = Create();
        using (provider) {
            provider.GetRequiredService<IFacilityClient>().ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_UseDefaultEndpoint_When_NoConfigurationProvided() {
        var (provider, handler, client) = Create();
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, "[]");

            await client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/facilities");
        }
    }

    [Theory]
    [InlineData("https://api.example.com/", "https://api.example.com/facilities")]
    [InlineData("https://example.org/fasta/v2/", "https://example.org/fasta/v2/facilities")]
    public async Task GetFacilitiesAsync_Should_UseConfiguredEndpoint_When_ConfigurationProvided(string endpoint, string expectedUri) {
        var (provider, handler, client) = Create(new Uri(endpoint));
        await using (provider) {
            handler.RespondWith(HttpStatusCode.OK, "[]");

            await client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

            handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe(expectedUri);
        }
    }

    [Fact]
    public async Task ValidateAsync_Should_RejectRelativeEndpoint_When_ValidationRuns() {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddDotBahnFacilities(options => options.BaseEndpoint = new Uri("relative", UriKind.Relative));
        await using var provider = services.BuildServiceProvider();
        var validator = provider.GetRequiredService<IAsyncStartupValidator>();

        var exception = await Should.ThrowAsync<OptionsValidationException>(() => validator.ValidateAsync());

        exception.Message.ShouldContain("DotBahn: BaseEndpoint must be an absolute URI.");
    }

    private static (ServiceProvider Provider, FakeHttpHandler Handler, IFacilityClient Client) Create(Uri? endpoint = null) {
        var handler = new FakeHttpHandler();
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddSingleton<IAuthorization>(new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        }));
        services.AddDotBahnFacilities(options => {
            if (endpoint != null) {
                options.BaseEndpoint = endpoint;
            }
        });
        services.AddHttpClient("DotBahn.Facilities").ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();
        return (provider, handler, provider.GetRequiredService<IFacilityClient>());
    }
}
