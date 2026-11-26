namespace DotBahn.Tests.Shared;

/// <summary>
/// Shared fixture that provides an <see cref="HttpClient"/> wired to a configurable <see cref="FakeHttpHandler"/>.
/// </summary>
public sealed class HttpTestFixture : IDisposable {
    /// <summary>
    /// Gets the fake HTTP handler used to stub responses.
    /// </summary>
    public FakeHttpHandler Handler { get; } = new();

    /// <summary>
    /// Gets the HTTP client configured with the fake handler.
    /// </summary>
    public HttpClient Client { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpTestFixture"/> class.
    /// </summary>
    public HttpTestFixture() {
        Client = new HttpClient(Handler, disposeHandler: false) {
            BaseAddress = new Uri("https://api.deutschebahn.com")
        };
    }

    /// <inheritdoc />
    public void Dispose() {
        Client.Dispose();
        Handler.Dispose();
    }
}
