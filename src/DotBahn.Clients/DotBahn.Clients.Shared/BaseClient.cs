using System.Net.Http.Headers;
using DotBahn.Modules.Auth.Service.Base;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Clients.Shared;

public abstract class BaseClient(HttpClient httpClient, IAuthorizationProvider authorizationProvider, ICacheProvider? cacheProvider = null) : IDisposable {
    protected async Task<TContract> GetAsync<TContract>(string relativeUrl, IParser<TContract> parser, string acceptHeader = "application/xml", TimeSpan? expiration = null) {
        var rawData = await GetContractDataAsync(relativeUrl, acceptHeader, expiration);
        return parser.Parse(rawData);
    }
    
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
