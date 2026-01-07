using System.Net.Http.Headers;
using DotBahn.Modules.Auth.Base;
using DotBahn.Modules.Cache.Base;

namespace DotBahn.Clients.Shared;

/// <summary>
/// Base class for Deutsche Bahn API clients
/// </summary>
public abstract class BaseClient(IAuthorizationProvider tokenService, HttpClient httpClient, ICacheProvider? cache = null) : IDisposable {
    protected readonly IAuthorizationProvider TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    protected readonly HttpClient HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    protected readonly ICacheProvider? Cache = cache;

    /// <summary>
    /// Helper method to fetch data from a specific endpoint
    /// </summary>
    protected async Task<string> GetRawDataAsync(string relativeUrl, string acceptHeader = "application/xml", TimeSpan? expiration = null) {
        var cacheKey = $"{relativeUrl}_{acceptHeader}";
        
        if (Cache != null) {
            var cachedData = await Cache.GetAsync<string>(cacheKey);
            if (cachedData != null) {
                return cachedData;
            }
        }

        var token = await TokenService.GetAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var rawData = await response.Content.ReadAsStringAsync();

        if (Cache != null) {
            await Cache.SetAsync(cacheKey, rawData, expiration);
        }

        return rawData;
    }

    public virtual void Dispose() {
        // The caller should manage HttpClient if passed via constructor.
    }
}
