using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service;

namespace DotBahn.Tests.Stations.Authorization;

public class ApiKeyAuthorizationTests {
    [Fact]
    public void AuthorizeRequest_SetsClientIdHeader() {
        var auth = new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "my-client-id",
            ApiKey = "my-api-key"
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        auth.AuthorizeRequest(request);

        Assert.Equal("my-client-id", request.Headers.GetValues("DB-Client-Id").Single());
    }

    [Fact]
    public void AuthorizeRequest_SetsApiKeyHeader() {
        var auth = new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "my-client-id",
            ApiKey = "my-api-key"
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        auth.AuthorizeRequest(request);

        Assert.Equal("my-api-key", request.Headers.GetValues("DB-Api-Key").Single());
    }

    [Fact]
    public void AuthorizeRequest_WithNullRequest_ThrowsArgumentNullException() {
        var auth = new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "my-client-id",
            ApiKey = "my-api-key"
        });

        Assert.Throws<ArgumentNullException>(() => auth.AuthorizeRequest(null!));
    }

    [Fact]
    public void AuthorizeRequest_SetsBothClientIdAndApiKeyHeaders() {
        var auth = new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "my-client-id",
            ApiKey = "my-api-key"
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        auth.AuthorizeRequest(request);

        Assert.Equal("my-client-id", request.Headers.GetValues("DB-Client-Id").Single());
        Assert.Equal("my-api-key", request.Headers.GetValues("DB-Api-Key").Single());
    }
}
