using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DotBahn.Modules.Auth.Base;
using DotBahn.Modules.Auth.Models;
using DotBahn.Modules.Shared;
using Microsoft.Extensions.Options;

namespace DotBahn.Modules.Auth;

/// <summary>
/// Service for managing OAuth access tokens.
/// </summary>
public class TokenAuthorizationProvider(IOptions<AuthConfiguration> options, HttpClient httpClient) : IAuthorizationProvider {
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private OAuthToken? _currentToken;
    private DateTime _tokenExpiry;

    /// <inheritdoc />
    public async Task<string> GetAccessTokenAsync() {
        if (_currentToken != null && DateTime.UtcNow < _tokenExpiry) {
            return _currentToken.AccessToken;
        }

        const string tokenUrl = "https://api.deutschebahn.com/oauth/v1/token";
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{options.Value.ClientId}:{options.Value.ClientSecret}")
        );

        using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new StringContent(
            "grant_type=client_credentials",
            Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<OAuthToken>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        _currentToken = tokenData ?? throw new InvalidOperationException("Failed to deserialize OAuth token.");
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn - 30); // Buffer of 30 seconds

        return _currentToken.AccessToken;
    }
}
