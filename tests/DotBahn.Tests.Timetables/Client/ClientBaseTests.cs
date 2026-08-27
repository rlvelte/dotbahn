using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Utilities;
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

        var client = new TestClientBase(http, authorization);

        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_DIConstructor_WithNullHttpClient_ThrowsArgumentNullException() {
        var authorization = new Mock<IAuthorization>().Object;

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(null!, authorization));
    }

    [Fact]
    public void Constructor_DIConstructor_WithNullAuthorization_ThrowsArgumentNullException() {
        var http = new HttpClient();

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(http, null!));
    }

    [Fact]
    public void Constructor_ConvenienceConstructor_CreatesHttpClientWithBaseAddress() {
        var endpoint = new Uri("https://api.deutschebahn.com");
        var options = new ClientOptions { BaseEndpoint = endpoint };
        var auth = new AuthorizationOptions {
            ClientId = "test-client",
            ApiKey = "test-key"
        };

        using var client = new TestClientBase(options, auth);

        Assert.NotNull(client);
        Assert.NotNull(client.HttpClient);
        Assert.Equal(endpoint, client.HttpClient.BaseAddress);
    }

    [Fact]
    public void Constructor_ConvenienceConstructor_WithNullOptions_ThrowsArgumentNullException() {
        var auth = new AuthorizationOptions {
            ClientId = "test-client",
            ApiKey = "test-key"
        };

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(null!, auth));
    }

    [Fact]
    public void Constructor_ConvenienceConstructor_WithNullAuth_ThrowsArgumentNullException() {
        var options = new ClientOptions {
            BaseEndpoint = new Uri("https://api.deutschebahn.com")
        };

        Assert.Throws<ArgumentNullException>(() => new TestClientBase(options, null!));
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


    public static TheoryData<string, bool, bool> GetAsyncUrlQueryParamHandlingCases => new() {
        { "BL—has_params", true, true },
        { "C1—null_params", false, false },
        { "C2—empty_params", true, false },
    };

    [Theory]
    [MemberData(nameof(GetAsyncUrlQueryParamHandlingCases))]
    public async Task GetAsyncUrlQueryParamHandling(string _, bool createParams, bool addParam) {
        var client = CreateClient();
        var parser = new Mock<IParser<string>>().Object;
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        QueryParameters? queryParams = null;
        if (createParams) {
            queryParams = QueryParameters.Create();
            if (addParam)
                queryParams.Add("key", "val");
        }

        await client.GetAsync("/test", parser, "application/xml", queryParams, TestContext.Current.CancellationToken);
        var requestUri = HttpHandler.SentRequests[0].RequestUri!.ToString();

        if (addParam) {
            Assert.Contains("?key=val", requestUri);
        } else {
            Assert.DoesNotContain("?", requestUri);
        }
    }

    private TestClientBase CreateClient() => new(HttpClient, AuthorizationMock.Object);

    private sealed class TestClientBase : ClientBase {
        public TestClientBase(HttpClient http, IAuthorization authorization) : base(http, authorization) { }

        public TestClientBase(ClientOptions options, AuthorizationOptions auth) : base(options, auth) { }

        public new HttpClient HttpClient => base.HttpClient;

        public Task<string> GetAsync(string relativeUrl, IParser<string> parser, string acceptHeader, QueryParameters? queryParams = null, CancellationToken cancellation = default) =>
            base.GetAsync(relativeUrl, parser, acceptHeader, queryParams, cancellation);
    }
}
