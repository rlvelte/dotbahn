using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache.Service.Base;

using Moq;

namespace DotBahn.Tests.Shared;

/// <summary>
/// Base class for client tests, providing common mock setup and assertion helpers.
/// </summary>
public abstract class ClientTestBase : IDisposable {
    protected MockHttpHandler HttpHandler { get; } = new();
    protected HttpClient HttpClient { get; }
    protected Mock<IAuthorization> AuthorizationMock { get; } = new();
    protected Mock<ICache> CacheMock { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientTestBase"/> class.
    /// </summary>
    protected ClientTestBase() {
        HttpClient = new HttpClient(HttpHandler, disposeHandler: false);
    }

    /// <summary>
    /// Asserts that a request was sent with the specified method and URL.
    /// </summary>
    /// <param name="method">The expected HTTP method.</param>
    /// <param name="relativeUrl">The expected relative URL (without a base address).</param>
    /// <param name="acceptHeader">The expected Accept header value.</param>
    /// <param name="expectedCount">The expected number of matching requests (default: 1).</param>
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

    /// <inheritdoc />
    public void Dispose() {
        HttpClient.Dispose();
        HttpHandler.Dispose();
    }
}
