using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Utilities;
using DotBahn.Tests.Shared;
using NSubstitute;
using Shouldly;
using System.Net;
using System.Text.Json.Serialization.Metadata;
using DotBahn.Tests.Shared.Contracts;
using DotBahn.Tests.Shared.Http;

namespace DotBahn.Common.Tests.Clients;

public class ClientBaseTests {
    private static JsonTypeInfo<SimpleDto> SimpleTypeInfo => (JsonTypeInfo<SimpleDto>)TestsJsonContext.Default.GetTypeInfo(typeof(SimpleDto))!;
    private static JsonTypeInfo<PostRequestDto> PostRequestTypeInfo => (JsonTypeInfo<PostRequestDto>)TestsJsonContext.Default.GetTypeInfo(typeof(PostRequestDto))!;
    private static JsonTypeInfo<AlwaysNullDto> NullTypeInfo => (JsonTypeInfo<AlwaysNullDto>)TestsJsonContext.Default.GetTypeInfo(typeof(AlwaysNullDto))!;

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_HttpClientIsNull() {
        var authorization = Substitute.For<IAuthorization>();

        var exception = Should.Throw<ArgumentNullException>(() => new TestClientBase(null!, authorization));

        exception.ParamName.ShouldBe("http");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_AuthorizationIsNull() {
        using var client = new HttpClient();

        var exception = Should.Throw<ArgumentNullException>(() => new TestClientBase(client, null!));

        exception.ParamName.ShouldBe("authorization");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_OptionsIsNull() {
        var auth = new AuthorizationOptions {
            ClientId = "id",
            ApiKey = "key"
        };

        var exception = Should.Throw<ArgumentNullException>(() => new TestClientBase(null!, auth));

        exception.ParamName.ShouldBe("options");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_AuthorizationOptionsIsNull() {
        var options = new ClientOptions {
            BaseEndpoint = new Uri("https://api.example.com/")
        };

        var exception = Should.Throw<ArgumentNullException>(() => new TestClientBase(options, null!));

        exception.ParamName.ShouldBe("auth");
    }

    [Fact]
    public void Constructor_Should_SetBaseAddressAndUserAgent_When_UsingOptionsOverload() {
        var options = new ClientOptions {
            BaseEndpoint = new Uri("https://api.example.com/")
        };
        var auth = new AuthorizationOptions {
            ClientId = "id",
            ApiKey = "key"
        };
        var sut = new TestClientBase(options, auth);

        sut.SharedClient.BaseAddress.ShouldBe(new Uri("https://api.example.com/"));
        sut.SharedClient.DefaultRequestHeaders.UserAgent.ShouldContain(h => h.Product!.Name.Contains("DotBahn"));
    }

    [Fact]
    public async Task GetAsync_Should_ThrowArgumentNullException_When_ParserIsNull() {
        using var client = CreateHttpClient(out _);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);

        var exception = await Should.ThrowAsync<ArgumentNullException>(() => sut.GetAsyncWrapper<string>("path", null!, "application/json"));

        exception.ParamName.ShouldBe("parser");
    }

    [Fact]
    public async Task GetAsync_Should_AppendQueryParameters_When_Provided() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{}");
        var parser = Substitute.For<IParser<string>>();
        parser.Parse(Arg.Any<string>()).Returns("ok");

        await sut.GetAsyncWrapper("path", parser, "application/json", QueryParameters.Create().Add("a", "1"), TestContext.Current.CancellationToken);

        handler.SentRequests.ShouldHaveSingleItem();
        handler.SentRequests[0].RequestUri!.ToString().ShouldContain("?a=1");
    }

    [Fact]
    public async Task GetAsync_Should_NotAppendQuery_When_QueryParametersAreEmpty() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{}");
        var parser = Substitute.For<IParser<string>>();
        parser.Parse(Arg.Any<string>()).Returns("ok");

        await sut.GetAsyncWrapper("path", parser, "application/json", QueryParameters.Create().Add("a", null), TestContext.Current.CancellationToken);

        handler.SentRequests[0].RequestUri!.ToString().ShouldNotContain("?");
    }

    [Fact]
    public async Task GetAsync_Should_ReturnParsedResponse_When_Success() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{\"Name\":\"ok\",\"Value\":1}");
        var parser = new JsonParser<SimpleDto>(SimpleTypeInfo);

        var result = await sut.GetAsyncWrapper("path", parser, "application/json", ct: TestContext.Current.CancellationToken);

        result.Name.ShouldBe("ok");
        result.Value.ShouldBe(1);
    }

    [Fact]
    public async Task GetAsync_Should_ReturnDefaultContract_When_NotFound() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.NotFound, "not found");
        var parser = new JsonParser<SimpleDto>(SimpleTypeInfo);

        var result = await sut.GetAsyncWrapper("path", parser, "application/json", ct: TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Name.ShouldBeNull();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetAsync_Should_ThrowHttpRequestException_When_ApiReturnsError(HttpStatusCode statusCode) {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(statusCode, "bad");

        var exception = await Should.ThrowAsync<HttpRequestException>(() => sut.GetAsyncWrapper("path", Substitute.For<IParser<string>>(), "application/json"));

        exception.StatusCode.ShouldBe(statusCode);
    }

    [Fact]
    public async Task GetAsync_Should_CallAuthorization_ForOutgoingRequest() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{}");
        var parser = Substitute.For<IParser<string>>();
        parser.Parse(Arg.Any<string>()).Returns("ok");

        await sut.GetAsyncWrapper("path", parser, "application/json", ct: TestContext.Current.CancellationToken);

        authorization.Received(1).AuthorizeRequest(Arg.Any<HttpRequestMessage>());
    }

    [Fact]
    public async Task GetAsync_Should_SetAcceptHeader() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{}");
        var parser = Substitute.For<IParser<string>>();
        parser.Parse(Arg.Any<string>()).Returns("ok");

        await sut.GetAsyncWrapper("path", parser, "application/json", ct: TestContext.Current.CancellationToken);

        handler.SentRequests[0].Headers.Accept.ShouldContain(h => h.MediaType == "application/json");
    }

    [Fact]
    public async Task GetAsync_Should_ThrowOperationCanceledException_When_Canceled() {
        using var client = CreateHttpClient(out _);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() => sut.GetAsyncWrapper("path", Substitute.For<IParser<string>>(), "application/json", ct: cts.Token));
    }

    [Fact]
    public async Task PostAsync_Should_ThrowArgumentNullException_When_ParserIsNull() {
        using var client = CreateHttpClient(out _);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);

        var exception = await Should.ThrowAsync<ArgumentNullException>(() => sut.PostAsyncWrapper<PostRequestDto, string>("path", new PostRequestDto(), PostRequestTypeInfo, null!, "application/json"));

        exception.ParamName.ShouldBe("parser");
    }

    [Fact]
    public async Task PostAsync_Should_ThrowArgumentNullException_When_RequestTypeIsNull() {
        using var client = CreateHttpClient(out _);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        JsonTypeInfo<PostRequestDto>? requestType = null;

        var exception = await Should.ThrowAsync<ArgumentNullException>(() => sut.PostAsyncWrapper("path", new PostRequestDto(), requestType!, Substitute.For<IParser<string>>(), "application/json"));

        exception.ParamName.ShouldBe("requestType");
    }

    [Fact]
    public async Task PostAsync_Should_ReturnParsedResponse_When_Success() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{\"Name\":\"post\",\"Value\":2}");
        var parser = new JsonParser<SimpleDto>(SimpleTypeInfo);

        var result = await sut.PostAsyncWrapper("path", new PostRequestDto { Name = "request", Count = 5 }, PostRequestTypeInfo, parser, "application/json", TestContext.Current.CancellationToken);

        result.Name.ShouldBe("post");
    }

    [Fact]
    public async Task PostAsync_Should_SendJsonContent_WithCorrectMediaType() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{}");
        var parser = Substitute.For<IParser<string>>();
        parser.Parse(Arg.Any<string>()).Returns("ok");

        await sut.PostAsyncWrapper("path", new PostRequestDto { Name = "request", Count = 5 }, PostRequestTypeInfo, parser, "application/json", TestContext.Current.CancellationToken);

        handler.SentRequestContentTypes[0].ShouldBe("application/json");
        var body = handler.SentRequestBodies[0];
        body.ShouldContain("\"Name\":\"request\"");
        body.ShouldContain("\"Count\":5");
    }

    [Fact]
    public async Task PostAsync_Should_CallAuthorization_ForOutgoingRequest() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.OK, "{}");
        var parser = Substitute.For<IParser<string>>();
        parser.Parse(Arg.Any<string>()).Returns("ok");

        await sut.PostAsyncWrapper("path", new PostRequestDto(), PostRequestTypeInfo, parser, "application/json", TestContext.Current.CancellationToken);

        authorization.Received(1).AuthorizeRequest(Arg.Any<HttpRequestMessage>());
    }

    [Fact]
    public async Task PostAsync_Should_ThrowHttpRequestException_When_ApiReturnsError() {
        using var client = CreateHttpClient(out var handler);
        var authorization = Substitute.For<IAuthorization>();
        var sut = new TestClientBase(client, authorization);
        handler.RespondWith(HttpStatusCode.BadRequest, "invalid");

        var exception = await Should.ThrowAsync<HttpRequestException>(() => sut.PostAsyncWrapper("path", new PostRequestDto(), PostRequestTypeInfo, Substitute.For<IParser<string>>(), "application/json"));

        exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private static HttpClient CreateHttpClient(out FakeHttpHandler handler) {
        handler = new FakeHttpHandler();
        return new HttpClient(handler, disposeHandler: false) {
            BaseAddress = new Uri("https://api.example.com/")
        };
    }

    private sealed class TestClientBase : ClientBase {
        public TestClientBase(HttpClient http, IAuthorization authorization) : base(http, authorization) { }

        public TestClientBase(ClientOptions options, AuthorizationOptions auth) : base(options, auth) { }

        protected override string ApiName => "test";

        public HttpClient SharedClient => HttpClient;

        public Task<TContract> GetAsyncWrapper<TContract>(string relative, IParser<TContract> parser, string acceptHeader, QueryParameters? queryParams = null, CancellationToken ct = default)
            => GetAsync(relative, parser, acceptHeader, queryParams, ct);

        public Task<TResponse> PostAsyncWrapper<TRequest, TResponse>(string relative, TRequest request, JsonTypeInfo<TRequest> requestType, IParser<TResponse> parser, string acceptHeader, CancellationToken ct = default)
            => PostAsync(relative, request, requestType, parser, acceptHeader, ct);
    }
}
