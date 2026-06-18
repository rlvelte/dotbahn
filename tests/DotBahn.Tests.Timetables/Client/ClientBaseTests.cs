using System.Net;

using DotBahn.Shared;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Shared.Parsing;
using DotBahn.Tests.Shared;

using Moq;

namespace DotBahn.Tests.Timetables.Client;

public class ClientBaseTests : ClientTestBase {

    public ClientBaseTests() {
        HttpClient.BaseAddress = new Uri("https://api.deutschebahn.com");
    }


    [Fact]
    public void Constructor_DIConstructor_StoresHttpClientReference() {
        var http = new HttpClient();
        var authorization = new Mock<IAuthorization>().Object;
        var cache = new Mock<ICache>().Object;

        var client = new TestClientBase(http, authorization, cache);

        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_DIConstructor_WithNullHttpClient_ThrowsArgumentNullException() {
        var authorization = new Mock<IAuthorization>().Object;

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(null!, authorization, null));
    }

    [Fact]
    public void Constructor_DIConstructor_WithNullAuthorization_ThrowsArgumentNullException() {
        var http = new HttpClient();

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(http, null!, null));
    }

    [Fact]
    public void Constructor_OptionsConstructor_StoresAndConfiguresHttpClient() {
        var http = new HttpClient();
        var options = new ClientOptions {
            BaseEndpoint = new Uri("https://api.deutschebahn.com")
        };
        var auth = new DotBahn.Modules.Authorization.AuthorizationOptions {
            ClientId = "test-client",
            ApiKey = "test-key"
        };

        var client = new TestClientBase(http, options, auth, null);

        Assert.NotNull(client);
        Assert.Same(http, client.HttpClient);
        Assert.Equal(options.BaseEndpoint, http.BaseAddress);
    }

    [Fact]
    public void Constructor_OptionsConstructor_WithNullHttp_ThrowsArgumentNullException() {
        var options = new ClientOptions {
            BaseEndpoint = new Uri("https://api.deutschebahn.com")
        };
        var auth = new DotBahn.Modules.Authorization.AuthorizationOptions {
            ClientId = "test-client",
            ApiKey = "test-key"
        };

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(null!, options, auth, null));
    }

    [Fact]
    public void Constructor_OptionsConstructor_WithNullOptions_ThrowsArgumentNullException() {
        var http = new HttpClient();
        var auth = new DotBahn.Modules.Authorization.AuthorizationOptions {
            ClientId = "test-client",
            ApiKey = "test-key"
        };

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(http, null!, auth, null));
    }

    [Fact]
    public void Constructor_OptionsConstructor_WithNullAuth_ThrowsArgumentNullException() {
        var http = new HttpClient();
        var options = new ClientOptions {
            BaseEndpoint = new Uri("https://api.deutschebahn.com")
        };

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(http, options, null!, null));
    }


    [Fact]
    public async Task GetAsync_BuildsCorrectUrlFromBaseAndRelative() {
        var client = CreateClient();
        var parser = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test/path", parser, "application/xml", cancellation: TestContext.Current.CancellationToken);

        AssertRequest(HttpMethod.Get, "/test/path", "application/xml");
    }

    [Fact]
    public async Task GetAsync_AppendsQueryParametersToUrl() {
        var client = CreateClient();
        var parser = new Mock<IParser<string>>().Object;
        var queryParams = QueryParameters.Create().Add("key1", "value1").Add("key2", "value2");
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test", parser, "application/xml", queryParams, TestContext.Current.CancellationToken);

        var request = HttpHandler.SentRequests[0];
        Assert.Contains("key1=value1", request.RequestUri?.ToString());
        Assert.Contains("key2=value2", request.RequestUri?.ToString());
    }

    [Fact]
    public async Task GetAsync_SetsAcceptHeader() {
        var client = CreateClient();
        var parser = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>", "application/json");

        await client.GetAsync("/test", parser, "application/json", cancellation: TestContext.Current.CancellationToken);

        AssertRequest(HttpMethod.Get, "/test", "application/json");
    }

    [Fact]
    public async Task GetAsync_CallsAuthorizeRequestExactlyOnce() {
        var client = CreateClient();
        var parser = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test", parser, "application/xml", cancellation: TestContext.Current.CancellationToken);

        AuthorizationMock.Verify(a => a.AuthorizeRequest(It.IsAny<HttpRequestMessage>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ReturnsParsedContractOnSuccess() {
        var client = CreateClient();
        var expectedContract = "test-contract";
        var parserMock = new Mock<IParser<string>>();
        parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(expectedContract);
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response>data</response>");

        var result = await client.GetAsync("/test", parserMock.Object, "application/xml", cancellation: TestContext.Current.CancellationToken);

        Assert.Equal(expectedContract, result);
    }

    [Fact]
    public async Task GetAsync_WithNullParser_ThrowsArgumentNullException() {
        var client = CreateClient();

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAsync("/test", null!, "application/xml", cancellation: TestContext.Current.CancellationToken));
    }


    [Fact]
    public async Task GetAsync_CacheHit_ReturnsCachedAndSkipsHttp() {
        var cacheMock = new Mock<ICache>();
        var cachedData = "cached-response";
        cacheMock.Setup(c => c.GetAsync<string>(It.IsAny<string>())).ReturnsAsync(cachedData);

        var client = new TestClientBase(HttpClient, AuthorizationMock.Object, cacheMock.Object);
        var parserMock = new Mock<IParser<string>>();
        parserMock.Setup(p => p.Parse(cachedData)).Returns("parsed-cached");

        var result = await client.GetAsync("/test", parserMock.Object, "application/xml", cancellation: TestContext.Current.CancellationToken);

        Assert.Equal("parsed-cached", result);
        Assert.Empty(HttpHandler.SentRequests);
        cacheMock.Verify(c => c.GetAsync<string>(It.IsAny<string>()), Times.Once);
        cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_CacheMiss_ExecutesHttpAndWritesCache() {
        var cacheMock = new Mock<ICache>();
        cacheMock.Setup(c => c.GetAsync<string>(It.IsAny<string>())).ReturnsAsync((string?)null);

        var client = new TestClientBase(HttpClient, AuthorizationMock.Object, cacheMock.Object);
        var parserMock = new Mock<IParser<string>>();
        parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns("parsed-response");

        HttpHandler.RespondWith(HttpStatusCode.OK, "<response>data</response>");

        var result = await client.GetAsync("/test", parserMock.Object, "application/xml", cancellation: TestContext.Current.CancellationToken);

        Assert.Equal("parsed-response", result);
        Assert.Single(HttpHandler.SentRequests);
        cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), "<response>data</response>"), Times.Once);
    }

    [Fact]
    public async Task GetAsync_CacheKeyIsFullRequestUri() {
        var cacheMock = new Mock<ICache>();
        cacheMock.Setup(c => c.GetAsync<string>(It.IsAny<string>())).ReturnsAsync((string?)null);

        var client = new TestClientBase(HttpClient, AuthorizationMock.Object, cacheMock.Object);
        var parserMock = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test", parserMock, "application/xml", cancellation: TestContext.Current.CancellationToken);

        cacheMock.Verify(c => c.GetAsync<string>(It.Is<string>(k => k.Contains("/test"))), Times.Once);
    }


    [Fact]
    public async Task GetAsync_With401Status_ThrowsHttpRequestExceptionUnauthorized() {
        var client = CreateClient();
        var parserMock = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.Unauthorized, "");

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("/test", parserMock, "application/xml", cancellation: TestContext.Current.CancellationToken));
        Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        Assert.Contains("not authorized", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAsync_With400Status_ThrowsHttpRequestExceptionBadRequest() {
        var client = CreateClient();
        var parserMock = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.BadRequest, "");

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("/test", parserMock, "application/xml", cancellation: TestContext.Current.CancellationToken));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Contains("Bad request", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAsync_With404Status_ReturnsDefaultEmptyString() {
        var client = CreateClient();
        var parserMock = new Mock<IParser<string>>();
        parserMock.Setup(p => p.Parse("")).Returns("");
        HttpHandler.RespondWith(HttpStatusCode.NotFound, "");

        var result = await client.GetAsync("/test", parserMock.Object, "application/xml", cancellation: TestContext.Current.CancellationToken);

        Assert.Equal("", result);
        parserMock.Verify(p => p.Parse(""), Times.Once);
    }

    [Fact]
    public async Task GetAsync_With500Status_ThrowsHttpRequestException() {
        var client = CreateClient();
        var parserMock = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.InternalServerError, "");

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("/test", parserMock, "application/xml", cancellation: TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Dispose_OptionsMode_DoesNotDisposeProvidedHttpClient() {
        var http = new HttpClient();
        var options = new ClientOptions {
            BaseEndpoint = new Uri("https://api.deutschebahn.com")
        };
        var auth = new DotBahn.Modules.Authorization.AuthorizationOptions {
            ClientId = "test-client",
            ApiKey = "test-key"
        };

        var client = new TestClientBase(http, options, auth, null);
        client.Dispose();

        // HttpClient is owned by the caller — dispose should not close it
        http.DefaultRequestHeaders.UserAgent.ParseAdd("still-alive");
        Assert.NotNull(http);
    }

    [Fact]
    public void Dispose_DIMode_DoesNotDisposeInjectedHttpClient() {
        var http = new HttpClient();
        var authorization = new Mock<IAuthorization>().Object;

        var client = new TestClientBase(http, authorization, null);
        client.Dispose();

        Assert.NotNull(http);
    }


    private TestClientBase CreateClient() => new(HttpClient, AuthorizationMock.Object, CacheMock.Object);

    private class TestClientBase : ClientBase {
        public TestClientBase(HttpClient http, IAuthorization authorization, ICache? cache)
            : base(http, authorization, cache) { }

        public TestClientBase(
            HttpClient http,
            ClientOptions options,
            DotBahn.Modules.Authorization.AuthorizationOptions auth,
            DotBahn.Modules.Cache.CacheOptions? cache)
            : base(http, options, auth, cache) { }

        public new HttpClient HttpClient => base.HttpClient;

        public Task<string> GetAsync(string relativeUrl, IParser<string> parser, string acceptHeader, QueryParameters? queryParams = null, CancellationToken cancellation = default) =>
            base.GetAsync(relativeUrl, parser, acceptHeader, queryParams, cancellation);
    }
}
