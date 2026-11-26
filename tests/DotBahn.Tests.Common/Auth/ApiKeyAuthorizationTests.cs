using DotBahn.Common.Auth;
using Shouldly;

namespace DotBahn.Tests.Common.Auth;

public class ApiKeyAuthorizationTests {
    [Theory]
    [InlineData("client-1", "key-1")]
    [InlineData("another-client", "another-key")]
    public void AuthorizeRequest_Should_SetDbClientIdHeader_When_Called(string clientId, string apiKey) {
        var options = new AuthorizationOptions {
            ClientId = clientId,
            ApiKey = apiKey
        };
        var sut = new ApiKeyAuthorization(options);
        var request = new HttpRequestMessage();

        sut.AuthorizeRequest(request);

        request.Headers.ShouldContain(h => h.Key == "DB-Client-Id");
        request.Headers.GetValues("DB-Client-Id").Single().ShouldBe(clientId);
    }

    [Theory]
    [InlineData("client-1", "key-1")]
    [InlineData("another-client", "another-key")]
    public void AuthorizeRequest_Should_SetDbApiKeyHeader_When_Called(string clientId, string apiKey) {
        var options = new AuthorizationOptions {
            ClientId = clientId,
            ApiKey = apiKey
        };
        var sut = new ApiKeyAuthorization(options);
        var request = new HttpRequestMessage();

        sut.AuthorizeRequest(request);

        request.Headers.ShouldContain(h => h.Key == "DB-Api-Key");
        request.Headers.GetValues("DB-Api-Key").Single().ShouldBe(apiKey);
    }

    [Fact]
    public void AuthorizeRequest_Should_ThrowArgumentNullException_When_RequestIsNull() {
        var options = new AuthorizationOptions {
            ClientId = "client",
            ApiKey = "key"
        };
        var sut = new ApiKeyAuthorization(options);

        var exception = Should.Throw<ArgumentNullException>(() => sut.AuthorizeRequest(null!));

        exception.ParamName.ShouldBe("request");
    }

    [Theory]
    [InlineData("client-123", "secret-key")]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    public void AuthorizeRequest_Should_SetBothHeaders_When_Called(string clientId, string apiKey) {
        var options = new AuthorizationOptions {
            ClientId = clientId,
            ApiKey = apiKey
        };
        var sut = new ApiKeyAuthorization(options);
        var request = new HttpRequestMessage();

        sut.AuthorizeRequest(request);

        request.Headers.ShouldContain(h => h.Key == "DB-Client-Id");
        request.Headers.ShouldContain(h => h.Key == "DB-Api-Key");
        request.Headers.GetValues("DB-Client-Id").Single().ShouldBe(clientId);
        request.Headers.GetValues("DB-Api-Key").Single().ShouldBe(apiKey);
    }
}
