using System.Net.Http.Headers;
using DotBahn.Cache.Base;
using DotBahn.Core.Token.Base;

namespace DotBahn.Core.Client;

/// <summary>
/// Base class for Deutsche Bahn API clients
/// </summary>
public abstract class BaseClient(BaseClientConfiguration configuration, ITokenService tokenService, HttpClient httpClient, ICache? cache = null) : IDisposable {
    protected readonly BaseClientConfiguration Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    protected readonly ITokenService TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    protected readonly HttpClient HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    protected readonly ICache? Cache = cache;

    /// <summary>
    /// Helper method to fetch data from a specific endpoint
    /// </summary>
    protected async Task<string> GetRawDataAsync(string relativeUrl, string acceptHeader = "application/xml") {
        var cacheKey = $"{relativeUrl}_{acceptHeader}";
        
        if (Cache != null) {
            var cachedData = await Cache.GetAsync<string>(cacheKey);
            if (cachedData != null) {
                return cachedData;
            }
        }

        var token = await TokenService.GetAccessTokenAsync();
        var url = $"{Configuration.BaseUrl}{relativeUrl}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var rawData = await response.Content.ReadAsStringAsync();

        if (Cache != null) {
            await Cache.SetAsync(cacheKey, rawData, Configuration.CacheOptions.DefaultExpiration);
        }

        return rawData;
    }

    public virtual void Dispose() {
        // The caller should manage HttpClient if passed via constructor.
    }
}
