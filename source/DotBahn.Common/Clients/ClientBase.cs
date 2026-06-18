using System.Net;
using System.Net.Http.Headers;
using DotBahn.Common.Auth;
using DotBahn.Common.Parsing;
using DotBahn.Common.Utilities;

namespace DotBahn.Common.Clients;

/// <summary>
/// Base class for API clients providing common HTTP functionality.
/// </summary>
public abstract class ClientBase {
    private readonly IAuthorization _authorization;

    /// <summary>
    /// The HTTP client used for requests.
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Initializes a new instance with the specified HTTP client and authorization.
    /// </summary>
    /// <param name="http">The HTTP client.</param>
    /// <param name="authorization">The authorization provider.</param>
    protected ClientBase(HttpClient http, IAuthorization authorization) {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(authorization);

        HttpClient = http;
        _authorization = authorization;
    }

    /// <summary>
    /// Initializes a new instance with API key authorization from the specified options.
    /// </summary>
    /// <param name="http">The HTTP client.</param>
    /// <param name="options">The client options containing the base endpoint.</param>
    /// <param name="auth">The authorization options containing the API key.</param>
    protected ClientBase(HttpClient http, ClientOptions options, AuthorizationOptions auth) {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(auth);

        http.BaseAddress = options.BaseEndpoint;
        http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/2.0 (+https://github.com/rlvelte/dotbahn)");

        HttpClient = http;
        _authorization = new ApiKeyAuthorization(auth);
    }

    /// <summary>
    /// Performs a GET request and parses the response into the specified contract type.
    /// </summary>
    /// <typeparam name="TContract">The type to parse the response into.</typeparam>
    /// <param name="relative">The relative request URL.</param>
    /// <param name="parser">The parser for deserializing the response.</param>
    /// <param name="acceptHeader">The accept header value.</param>
    /// <param name="queryParams">Optional query parameters.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>The parsed response.</returns>
    protected async Task<TContract> GetAsync<TContract>(string relative, IParser<TContract> parser, string acceptHeader, QueryParameters? queryParams = null, CancellationToken cancellation = default) {
        ArgumentNullException.ThrowIfNull(parser);

        var url = queryParams is null || !queryParams.Any() ? relative : $"{relative}?{queryParams.ToQueryString()}";

        var requestUri = BuildRequestUri(url);
        var raw = await ExecuteHttpRequestAsync(requestUri, acceptHeader, cancellation).ConfigureAwait(false);
        return parser.Parse(raw);
    }

    /// <summary>
    /// Combines the base address with a relative URL to produce an absolute or relative URI.
    /// </summary>
    /// <param name="relativeUrl">The relative URL path.</param>
    /// <returns>An absolute URI if a base address is configured; otherwise a relative URI.</returns>
    private Uri BuildRequestUri(string relativeUrl) {
        var path = relativeUrl.TrimStart('/');
        var baseUrl = (HttpClient.BaseAddress?.AbsoluteUri ?? "").TrimEnd('/');

        return baseUrl.Length > 0 ? new Uri($"{baseUrl}/{path}") : new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Sends an authorized GET request and processes the response.
    /// </summary>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="acceptHeader">The acceptance header value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response body as a string.</returns>
    private async Task<string> ExecuteHttpRequestAsync(Uri requestUri, string acceptHeader, CancellationToken cancellationToken) {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        _authorization.AuthorizeRequest(request);

        using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync(response).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes the HTTP response, returning the body for success status codes or throwing on errors.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>The response body for successful requests; <c>string.Empty</c> for 404.</returns>
    /// <exception cref="HttpRequestException">Thrown for non-success status codes with a descriptive message.</exception>
    private static async Task<string> ProcessResponseAsync(HttpResponseMessage response) {
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return response.StatusCode switch {
            HttpStatusCode.NotFound => string.Empty,
            HttpStatusCode.Unauthorized =>
                throw new HttpRequestException("Request was not authorized.", null, response.StatusCode),
            HttpStatusCode.BadRequest =>
                throw new HttpRequestException($"Bad request: {DescribeBody(body)}", null, response.StatusCode),
            HttpStatusCode.Forbidden =>
                throw new HttpRequestException("Access denied.", null, response.StatusCode),
            HttpStatusCode.TooManyRequests =>
                throw new HttpRequestException("Rate limit exceeded.", null, response.StatusCode),
            _ when !response.IsSuccessStatusCode =>
                throw new HttpRequestException($"The API responded with status {(int)response.StatusCode}: {DescribeBody(body)}", null, response.StatusCode),
            _ => body
        };
    }

    /// <summary>
    /// Truncates the response body for inclusion in exception messages.
    /// </summary>
    /// <param name="body">The raw response body.</param>
    /// <returns>The body unchanged if 200 characters or fewer; otherwise the first 200 characters with an ellipsis.</returns>
    private static string DescribeBody(string body) => string.IsNullOrEmpty(body) ? string.Empty : body.Length <= 200 ? body : body[..200] + "...";
}
