using System.Net;

namespace DotBahn.Tests.Shared;

/// <summary>
/// Configurable HttpMessageHandler that returns predefined responses and captures requests for verification.
/// </summary>
public class FakeHttpHandler : HttpMessageHandler {
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responses = new();

    /// <summary>
    /// Gets the list of all sent HTTP requests.
    /// </summary>
    public List<HttpRequestMessage> SentRequests { get; } = [];

    /// <summary>
    /// Configures the handler to respond with a specific status code and content.
    /// </summary>
    /// <param name="status">The HTTP status code to return.</param>
    /// <param name="content">The response content body.</param>
    /// <param name="contentType">The content type header value.</param>
    public void RespondWith(HttpStatusCode status, string content, string contentType = "application/json") {
        _responses.Enqueue(_ => new HttpResponseMessage(status) {
            Content = new StringContent(content, System.Text.Encoding.UTF8, contentType)
        });
    }

    /// <summary>
    /// Configures the handler to respond with a dynamically generated response.
    /// </summary>
    /// <param name="factory">A function that takes the request and returns a response.</param>
    public void RespondWith(Func<HttpRequestMessage, HttpResponseMessage> factory) {
        _responses.Enqueue(factory);
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) {
        SentRequests.Add(request);

        if (_responses.Count == 0) {
            throw new InvalidOperationException("No response configured.");
        }

        var factory = _responses.Dequeue();
        return Task.FromResult(factory(request));
    }
}
