using System.Net;

namespace DotBahn.Common.Tests;

/// <summary>
/// Configurable HttpMessageHandler that returns predefined responses
/// and captures requests for verification.
/// </summary>
public class MockHttpHandler : HttpMessageHandler {
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responseFactories = new();

    /// <summary>
    /// Gets the list of all sent HTTP requests.
    /// </summary>
    public List<HttpRequestMessage> SentRequests { get; } = new();

    /// <summary>
    /// Configures the handler to respond with a specific status code and content.
    /// </summary>
    /// <param name="status">The HTTP status code to return.</param>
    /// <param name="content">The response content body.</param>
    /// <param name="contentType">The content type header value (default: "application/xml").</param>
    /// <returns>The handler instance for fluent chaining.</returns>
    public void RespondWith(HttpStatusCode status, string content, string contentType = "application/xml") {
        _responseFactories.Enqueue(_ => new HttpResponseMessage(status) {
            Content = new StringContent(content, System.Text.Encoding.UTF8, contentType)
        });
    }

    /// <summary>
    /// Configures the handler to respond with a dynamically generated response.
    /// </summary>
    /// <param name="factory">A function that takes the request and returns a response.</param>
    /// <returns>The handler instance for fluent chaining.</returns>
    public void RespondWith(Func<HttpRequestMessage, HttpResponseMessage> factory) {
        _responseFactories.Enqueue(factory);
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        SentRequests.Add(request);

        if (_responseFactories.Count == 0) {
            throw new InvalidOperationException("No response configured. Use RespondWith() to set up a response.");
        }

        var factory = _responseFactories.Dequeue();
        return Task.FromResult(factory(request));
    }
}
