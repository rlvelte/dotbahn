using DotBahn.Common.Auth;
using Moq;

namespace DotBahn.Tests.Shared;

public abstract class ClientTestBase : IDisposable {
    protected MockHttpHandler HttpHandler { get; } = new();
    protected HttpClient HttpClient { get; }
    protected Mock<IAuthorization> AuthorizationMock { get; } = new();

    protected ClientTestBase() {
        HttpClient = new HttpClient(HttpHandler, disposeHandler: false);
    }

    protected void AssertRequest(HttpMethod method, string relativeUrl, string? acceptHeader = null, int? expectedCount = 1) {
        var count = expectedCount ?? 1;
        var matchingRequests = HttpHandler.SentRequests
            .Where(r => r.Method == method && r.RequestUri?.ToString().EndsWith(relativeUrl.TrimStart('/'), StringComparison.Ordinal) == true)
            .ToList();

        Assert.Equal(count, matchingRequests.Count);

        if (acceptHeader == null || matchingRequests.Count <= 0) {
            return;
        }

        var hasAcceptHeader = matchingRequests[0].Headers.Accept.Any(h => h.MediaType == acceptHeader);
        Assert.True(hasAcceptHeader, $"Expected Accept header '{acceptHeader}' not found. Actual headers: {string.Join(", ", matchingRequests[0].Headers.Accept.Select(h => h.MediaType))}");
    }

    public void Dispose() {
        HttpClient.Dispose();
        HttpHandler.Dispose();
    }
}
