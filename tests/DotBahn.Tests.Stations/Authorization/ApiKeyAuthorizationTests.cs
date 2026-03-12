using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service;

namespace DotBahn.Tests.Stations.Authorization;

public class ApiKeyAuthorizationTests {
    [Fact]
    public void AuthorizeRequest_SetsClientIdHeader() {
        // Arrange
        var auth = new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "my-client-id",
            ApiKey = "my-api-key"
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // Act
        auth.AuthorizeRequest(request);

        // Assert
        Assert.Equal("my-client-id", request.Headers.GetValues("DB-Client-Id").Single());
    }

    [Fact]
    public void AuthorizeRequest_SetsApiKeyHeader() {
        // Arrange
        var auth = new ApiKeyAuthorization(new AuthorizationOptions {
            ClientId = "my-client-id",
            ApiKey = "my-api-key"
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // Act
        auth.AuthorizeRequest(request);

        // Assert
        Assert.Equal("my-api-key", request.Headers.GetValues("DB-Api-Key").Single());
    }
}
