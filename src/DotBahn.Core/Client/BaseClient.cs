using System.Net.Http.Headers;
using DotBahn.Core.Token.Base;

namespace DotBahn.Core.Client;

/// <summary>
/// Base class for Deutsche Bahn API clients
/// </summary>
public abstract class BaseClient(BaseClientConfiguration configuration, ITokenService tokenService, HttpClient httpClient) : IDisposable {
    protected readonly BaseClientConfiguration Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    protected readonly ITokenService TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    protected readonly HttpClient HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Helper method to fetch data from a specific endpoint
    /// </summary>
    protected async Task<string> GetRawDataAsync(string relativeUrl, string acceptHeader = "application/xml") {
        var token = await TokenService.GetAccessTokenAsync();
        var url = $"{Configuration.BaseUrl}{relativeUrl}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public virtual void Dispose() {
        // HttpClient should be managed by the caller if passed via constructor.
    }
}
