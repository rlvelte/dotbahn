using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Utilities;
using DotBahn.Common.Tests;
using Moq;

namespace DotBahn.Timetables.Tests.Client;

public class UriUtilTests : ClientTestBase {
    private readonly Mock<IParser<string>> _parserMock = new();

    public UriUtilTests() {
        HttpClient.BaseAddress = new Uri("https://api.deutschebahn.com");
    }

    [Fact]
    public async Task GetAsync_WithNullQueryParameters_NoQueryStringInUrl() {
        var client = CreateClient();
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test", _parserMock.Object, "application/xml", cancellation: TestContext.Current.CancellationToken);

        var requestUri = HttpHandler.SentRequests[0].RequestUri!.ToString();
        Assert.DoesNotContain("?", requestUri);
    }

    [Fact]
    public async Task GetAsync_WithEmptyQueryParameters_NoQueryStringInUrl() {
        var client = CreateClient();
        var queryParams = QueryParameters.Create();
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test", _parserMock.Object, "application/xml", queryParams, TestContext.Current.CancellationToken);

        var requestUri = HttpHandler.SentRequests[0].RequestUri!.ToString();
        Assert.DoesNotContain("?", requestUri);
    }

    [Fact]
    public async Task GetAsync_WithQueryParameters_AppendsQueryStringToUrl() {
        var client = CreateClient();
        var queryParams = QueryParameters.Create()
            .Add("key1", "value1")
            .Add("key2", "value2");
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test", _parserMock.Object, "application/xml", queryParams, TestContext.Current.CancellationToken);

        var requestUri = HttpHandler.SentRequests[0].RequestUri!.ToString();
        Assert.Contains("?key1=value1", requestUri);
        Assert.Contains("&key2=value2", requestUri);
    }

    [Fact]
    public async Task GetAsync_WithSpecialCharacters_UrlEncodesParameters() {
        var client = CreateClient();
        var queryParams = QueryParameters.Create().Add("search", "test value");
        HttpHandler.RespondWith(HttpStatusCode.OK, "<response/>");

        await client.GetAsync("/test", _parserMock.Object, "application/xml", queryParams, TestContext.Current.CancellationToken);

        Assert.Single(HttpHandler.SentRequests);
        var requestUri = HttpHandler.SentRequests[0].RequestUri!.AbsoluteUri;
        Assert.Contains("%20", requestUri);
    }

    private TestClientBase CreateClient() => new(HttpClient, AuthorizationMock.Object);

    private class TestClientBase(HttpClient http, IAuthorization authorization) : ClientBase(http, authorization) {
        public Task<string> GetAsync(string relativeUrl, IParser<string> parser, string acceptHeader, QueryParameters? queryParams = null, CancellationToken cancellation = default) =>
            base.GetAsync(relativeUrl, parser, acceptHeader, queryParams, cancellation);
    }
}
