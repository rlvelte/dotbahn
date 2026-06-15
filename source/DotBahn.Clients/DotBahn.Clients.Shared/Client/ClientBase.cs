using System.Net;
using System.Net.Http.Headers;

using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Shared.Query;
using DotBahn.Clients.Shared.Utilities;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Service;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Shared.Client;

/// <summary>
/// Base class for rest clients, providing common functionality for authentication and request caching.
/// </summary>
public abstract class ClientBase : IDisposable {
    private readonly HttpClient _http;
    private readonly IAuthorization _authorization;
    private readonly ICache? _cache;

    /// <summary>
    /// Base class for rest clients, providing common functionality for authentication and request caching.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    protected ClientBase(HttpClient http, IAuthorization authorization, ICache? cache) {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(authorization);

        _http = http;
        _authorization = authorization;
        _cache = cache;
    }

    /// <summary>
    /// Base class for rest clients, providing common functionality for authentication and request caching.
    /// </summary>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <param name="cache">The cache options for the client.</param>
    protected ClientBase(ClientOptions options, AuthorizationOptions auth, CacheOptions? cache = null) {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(auth);

        _http = new HttpClient(new SocketsHttpHandler {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        }) {
            BaseAddress = options.BaseEndpoint,
        };

        _http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");

        _authorization = new ApiKeyAuthorization(auth);

        if (cache == null) {
            return;
        }

        _cache = new InMemoryCache(cache);
    }

    /// <inheritdoc />
    public void Dispose() {
        _http.Dispose();
        _cache?.Dispose();
    }

    /// <summary>
    /// Sends a GET request to the specified relative URL and parses the response.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract to parse.</typeparam>
    /// <param name="relativeUrl">The relative URL for the request.</param>
    /// <param name="parser">The parser used to convert the raw response to the contract.</param>
    /// <param name="acceptHeader">The value for the Accept header.</param>
    /// <param name="queryParams">Optional query parameters.</param>
    /// <param name="cancellation">Optional cancellation token.</param>
    /// <returns>The parsed contract.</returns>
    protected async Task<TContract> GetAsync<TContract>(string relativeUrl, IParser<TContract> parser, string acceptHeader, QueryParameters? queryParams = null, CancellationToken cancellation = default) {
        ArgumentNullException.ThrowIfNull(parser);

        var url = UriUtil.BuildUrl(relativeUrl, queryParams);
        var raw = await GetContractDataAsync(url, acceptHeader, cancellation).ConfigureAwait(false);
        return parser.Parse(raw);
    }

    /// <summary>
    /// Retrieves contract data from the API or cache.
    /// </summary>
    /// <param name="url">The relative URL including query parameters.</param>
    /// <param name="acceptHeader">The value for the Accept header.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>The raw response data or an empty string if the resource was not found.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    private async Task<string> GetContractDataAsync(string url, string acceptHeader, CancellationToken cancellation) {
        var requestUri = BuildRequestUri(url);

        if (_cache != null) {
            var cachedData = await _cache.GetAsync<string>(requestUri.ToString()).ConfigureAwait(false);
            if (cachedData != null) {
                return cachedData;
            }
        }

        var responseData = await ExecuteHttpRequestAsync(requestUri, acceptHeader, cancellation).ConfigureAwait(false);
        if (_cache != null) {
            await _cache.SetAsync(requestUri.ToString(), responseData).ConfigureAwait(false);
        }

        return responseData;
    }

    /// <summary>
    /// Builds the complete request URI from relative URL and the configured base address.
    /// </summary>
    private Uri BuildRequestUri(string relativeUrl) {
        var path = relativeUrl.TrimStart('/');
        var baseUrl = (_http.BaseAddress?.AbsoluteUri ?? "").TrimEnd('/');

        return baseUrl.Length > 0 ? new Uri($"{baseUrl}/{path}") : new Uri(path, UriKind.Relative);
    }

    /// <summary>
    /// Executes the HTTP GET request and handles status codes.
    /// </summary>
    private async Task<string> ExecuteHttpRequestAsync(Uri requestUri, string acceptHeader, CancellationToken cancellationToken) {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        _authorization.AuthorizeRequest(request);

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync(response).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes the response from the server and checks status.
    /// </summary>
    /// <param name="response">The response the client got.</param>
    /// <returns>The response content, if available.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    private static async Task<string> ProcessResponseAsync(HttpResponseMessage response) =>
        response.StatusCode switch {
            HttpStatusCode.Unauthorized =>
                throw new HttpRequestException("Request was not authorized.", null, response.StatusCode),
            HttpStatusCode.BadRequest =>
                throw new HttpRequestException("Bad request.", null, response.StatusCode),

            HttpStatusCode.NotFound => string.Empty,
            _ => await ReadSuccessResponseAsync(response).ConfigureAwait(false)
        };

    /// <summary>
    /// Ensures a successful request and reads the response content.
    /// </summary>
    /// <param name="response">The response to read from.</param>
    /// <returns>The content of the response.</returns>
    private static async Task<string> ReadSuccessResponseAsync(HttpResponseMessage response) {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}
