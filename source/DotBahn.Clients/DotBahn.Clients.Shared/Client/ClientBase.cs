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
using Microsoft.Extensions.Caching.Memory;

namespace DotBahn.Clients.Shared.Client;

/// <summary>
/// Base class for rest clients, providing common functionality for authentication and request caching.
/// </summary>
public abstract class ClientBase {
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
        _http = new HttpClient {
            BaseAddress = options.BaseEndpoint,
        };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
        
        _authorization = new ApiKeyAuthorization(auth);
        if (cache == null) {
            return;
        }

        var memoryCache = new MemoryCache(new MemoryCacheOptions {
            SizeLimit = 1024
        });
        _cache = new InMemoryCache(memoryCache, cache);
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
        var url = UriUtil.BuildUrl(relativeUrl, queryParams);
        
        var raw = await GetContractDataAsync(url, acceptHeader, cancellation);
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
            var cachedData = await _cache.GetAsync<string>(requestUri.ToString());
            if (cachedData != null) {
                return cachedData;
            }
        }

        var responseData = await ExecuteHttpRequestAsync(requestUri, acceptHeader, cancellation);
        if (_cache != null) {
            await _cache.SetAsync(requestUri.ToString(), responseData);
        }

        return responseData;
    }

    /// <summary>
    /// Builds the complete request URI from relative URL.
    /// </summary>
    private Uri BuildRequestUri(string relativeUrl) {
        var path = relativeUrl.TrimStart('/');
        
        if (_http.BaseAddress == null) {
            return new Uri(path, UriKind.RelativeOrAbsolute);
        }
        
        var url = _http.BaseAddress.ToString();
        return new Uri(url.EndsWith('/') ? _http.BaseAddress : new Uri(url + "/"), path);
    }

    /// <summary>
    /// Executes the HTTP GET request and handles status codes.
    /// </summary>
    private async Task<string> ExecuteHttpRequestAsync(Uri requestUri, string acceptHeader, CancellationToken cancellationToken) {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        
        _authorization.AuthorizeRequest(request);
        
        using var response = await _http.SendAsync(request, cancellationToken);
        return await ProcessResponseAsync(response);
    }

    /// <summary>
    /// Processes the response from the server and chack status.
    /// </summary>
    /// <param name="response">The response the client got.</param>
    /// <returns>The response content, if available.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    private static async Task<string> ProcessResponseAsync(HttpResponseMessage response) {
        return response.StatusCode switch {
            HttpStatusCode.Unauthorized => 
                throw new HttpRequestException("Request was not authorized.", null, response.StatusCode),
            HttpStatusCode.BadRequest =>
                throw new HttpRequestException("Bad request.", null, response.StatusCode),
            
            HttpStatusCode.NotFound => string.Empty,
            _ => await ReadSuccessResponseAsync(response)
        };
    }

    /// <summary>
    /// Ensures a successful request and reads the response content.
    /// </summary>
    /// <param name="response">The response to read from.</param>
    /// <returns>The content of the response.</returns>
    private static async Task<string> ReadSuccessResponseAsync(HttpResponseMessage response) {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}