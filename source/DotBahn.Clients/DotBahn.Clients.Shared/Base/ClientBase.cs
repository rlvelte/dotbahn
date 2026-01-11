using System.Net;
using System.Net.Http.Headers;
using DotBahn.Clients.Shared.Models;
using DotBahn.Clients.Shared.Utilities;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Shared.Base;

/// <summary>
/// Base class for rest clients, providing common functionality for authentication and request caching.
/// </summary>
/// <param name="http">The HTTP client used for requests.</param>
/// <param name="authorization">The provider used for retrieving access tokens.</param>
/// <param name="cache">The cache provider for storing requests.</param>
public abstract class ClientBase(HttpClient http, IAuthorization authorization, ICache cache) {
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
        
        var cachedData = await cache.GetAsync<string>(requestUri.ToString());
        if (cachedData != null) {
            return cachedData;
        }
        
        var responseData = await ExecuteHttpRequestAsync(requestUri, acceptHeader, cancellation);
        if (!string.IsNullOrEmpty(responseData)) {
            await cache.SetAsync(requestUri.ToString(), responseData);
        }
        
        return responseData;
    }

    /// <summary>
    /// Builds the complete request URI from relative URL.
    /// </summary>
    private Uri BuildRequestUri(string relativeUrl) {
        var path = relativeUrl.TrimStart('/');
        
        if (http.BaseAddress == null) {
            return new Uri(path, UriKind.RelativeOrAbsolute);
        }
        
        var url = http.BaseAddress.ToString();
        return new Uri(url.EndsWith('/') ? http.BaseAddress : new Uri(url + "/"), path);
    }

    /// <summary>
    /// Executes the HTTP GET request and handles status codes.
    /// </summary>
    private async Task<string> ExecuteHttpRequestAsync(Uri requestUri, string acceptHeader, CancellationToken cancellationToken) {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        
        await authorization.AuthorizeRequestAsync(request);
        
        using var response = await http.SendAsync(request, cancellationToken);
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