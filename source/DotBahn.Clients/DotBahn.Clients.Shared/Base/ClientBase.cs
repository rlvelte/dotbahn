using System.Net.Http.Headers;
using DotBahn.Clients.Shared.Queries;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.RequestCache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Shared.Base;

/// <summary>
/// Base class for rest clients, providing common functionality for authentication and request caching.
/// </summary>
/// <param name="http">The HTTP client used for requests.</param>
/// <param name="authorization">The provider used for retrieving access tokens.</param>
/// <param name="cache">The cache provider for storing responses.</param>
public abstract class ClientBase(HttpClient http, IAuthorizationProvider authorization, IRequestCache cache) : IDisposable {
    /// <summary>
    /// Sends a GET request to the specified relative URL and parses the response.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract to parse.</typeparam>
    /// <param name="relativeUrl">The relative URL for the request.</param>
    /// <param name="parser">The parser used to convert the raw response to the contract.</param>
    /// <param name="acceptHeader">The value for the Accept header.</param>
    /// <param name="queryParams">Optional query parameters.</param>
    /// <returns>The parsed contract.</returns>
    protected async Task<TContract> GetAsync<TContract>(string relativeUrl, IParser<TContract> parser, string acceptHeader, QueryParameters? queryParams = null) {
        var url = BuildUrl(relativeUrl, queryParams);
        var rawData = await GetContractDataAsync(url, acceptHeader);

        return parser.Parse(rawData);
    }
    
    /// <summary>
    /// Sends multiple GET requests in parallel and parses the responses.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract to parse.</typeparam>
    /// <param name="relativeUrls">The list of relative URLs for the requests.</param>
    /// <param name="parser">The parser used to convert the raw responses to the contracts.</param>
    /// <param name="acceptHeader">The value for the Accept header.</param>
    /// <returns>A collection of parsed contracts.</returns>
    protected async Task<IEnumerable<TContract>> GetBatchAsync<TContract>(IEnumerable<string> relativeUrls, IParser<TContract> parser, string acceptHeader) {
        var tasks = relativeUrls.Select(url => GetAsync(url, parser, acceptHeader));
        return await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Builds a URL with query parameters.
    /// </summary>
    /// <param name="relativeUrl">The base relative URL.</param>
    /// <param name="queryParams">Optional query parameters.</param>
    /// <returns>The complete URL with query string.</returns>
    private static string BuildUrl(string relativeUrl, QueryParameters? queryParams) {
        if (queryParams == null || !queryParams.Any()) {
            return relativeUrl;
        }
        
        var queryString = queryParams.ToQueryString();
        return string.IsNullOrEmpty(queryString) ? relativeUrl : $"{relativeUrl}?{queryString}";
    }
    
    /// <summary>
    /// Retrieves contract data from the API or cache.
    /// </summary>
    /// <param name="relativeUrl">The relative URL including query parameters.</param>
    /// <param name="acceptHeader">The value for the Accept header.</param>
    /// <returns>The raw response data.</returns>
    private async Task<string> GetContractDataAsync(string relativeUrl, string acceptHeader) {
        var cacheKey = $"{relativeUrl}_{acceptHeader}";
        var cachedData = await cache.GetAsync<string>(cacheKey);
        if (cachedData != null) {
            return cachedData;
        }

        var path = relativeUrl.StartsWith('/') ? relativeUrl[1..] : relativeUrl;
        Uri requestUri;

        if (http.BaseAddress != null) {
            var baseUriStr = http.BaseAddress.ToString();
            if (!baseUriStr.EndsWith('/')) {
                baseUriStr += "/";
            }
            requestUri = new Uri(new Uri(baseUriStr), path);
        } else {
            requestUri = new Uri(path, UriKind.RelativeOrAbsolute);
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        await authorization.AuthorizeRequestAsync(request);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();
        await cache.SetAsync(cacheKey, raw);
        
        return raw;
    }

    public virtual void Dispose() {
        // The caller should manage HttpClient if passed via constructor.
    }
}