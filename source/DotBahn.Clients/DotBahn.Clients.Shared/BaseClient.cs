using System.Net.Http.Headers;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Shared;

/// <summary>
/// Base class for API clients, providing common functionality for authentication and caching.
/// </summary>
/// <param name="httpClient">The HTTP client used for requests.</param>
/// <param name="authorizationProvider">The provider used for retrieving access tokens.</param>
/// <param name="cacheProvider">The optional cache provider for storing responses.</param>
public abstract class BaseClient(HttpClient httpClient, IAuthorizationProvider authorizationProvider, ICacheProvider? cacheProvider = null) : IDisposable {
    /// <summary>
    /// Sends a GET request to the specified relative URL and parses the response.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract to parse.</typeparam>
    /// <param name="relativeUrl">The relative URL for the request.</param>
    /// <param name="parser">The parser used to convert the raw response to the contract.</param>
    /// <param name="acceptHeader">The value for the Accept header.</param>
    /// <param name="expiration">Optional expiration time for the cached response.</param>
    /// <returns>The parsed contract.</returns>
    protected async Task<TContract> GetAsync<TContract>(string relativeUrl, IParser<TContract> parser, string acceptHeader = "application/xml", TimeSpan? expiration = null) {
        var rawData = await GetContractDataAsync(relativeUrl, acceptHeader, expiration);
        return parser.Parse(rawData);
    }
    
    /// <summary>
    /// Sends multiple GET requests in parallel and parses the responses.
    /// </summary>
    /// <typeparam name="TContract">The type of the contract to parse.</typeparam>
    /// <param name="relativeUrls">The list of relative URLs for the requests.</param>
    /// <param name="parser">The parser used to convert the raw responses to the contracts.</param>
    /// <param name="acceptHeader">The value for the Accept header.</param>
    /// <param name="expiration">Optional expiration time for the cached responses.</param>
    /// <returns>A collection of parsed contracts.</returns>
    protected async Task<IEnumerable<TContract>> GetBatchAsync<TContract>(IEnumerable<string> relativeUrls, IParser<TContract> parser, string acceptHeader = "application/xml", TimeSpan? expiration = null) {
        var tasks = relativeUrls.Select(url => GetAsync(url, parser, acceptHeader, expiration));
        return await Task.WhenAll(tasks);
    }
    
    private async Task<string> GetContractDataAsync(string relativeUrl, string acceptHeader = "application/xml", TimeSpan? expiration = null) {
        var cacheKey = $"{relativeUrl}_{acceptHeader}";
        if (cacheProvider != null) {
            var cachedData = await cacheProvider.GetAsync<string>(cacheKey);
            if (cachedData != null) {
                return cachedData;
            }
        }

        var token = await authorizationProvider.GetAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();

        if (cacheProvider != null) {
            await cacheProvider.SetAsync(cacheKey, raw, expiration);
        }

        return raw;
    }

    public virtual void Dispose() {
        // The caller should manage HttpClient if passed via constructor.
    }
}
