using System.Net.Http.Headers;
using System.Text;
using DotDB.Client.Configuration;
using DotDB.Client.Parser;
using DotDB.Models;

namespace DotDB.Client;

/// <summary>
/// Client for Deutsche Bahn Timetables API
/// </summary>
public class TimetableApiClient(DotBahnConfiguration configuration) : IDisposable {
    private readonly DotBahnConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly HttpClient _httpClient = new();
    private string _accessToken;
    private DateTime? _tokenExpiry;

    /// <summary>
    /// Gets OAuth access token
    /// </summary>
    private async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && 
            _tokenExpiry.HasValue && 
            DateTime.Now < _tokenExpiry.Value)
        {
            return _accessToken;
        }

        var tokenUrl = "https://api.deutschebahn.com/oauth/v1/token";
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_configuration.ClientId}:{_configuration.ClientSecret}")
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
        // Simple JSON parsing - use System.Text.Json or Newtonsoft.Json in production
        var tokenData = System.Text.Json.JsonSerializer.Deserialize<
            Dictionary<string, object>
        >(json);

        _accessToken = tokenData["access_token"].ToString();
        var expiresIn = int.Parse(tokenData["expires_in"].ToString());
        _tokenExpiry = DateTime.Now.AddSeconds(expiresIn);

        return _accessToken;
    }

    /// <summary>
    /// Fetches full timetable changes for a station
    /// </summary>
    /// <param name="evaNo">EVA station number (7-digit number starting with 80 for Germany)</param>
    public async Task<Timetable> GetFullChangesAsync(string evaNo)
    {
        var token = await GetAccessTokenAsync();
        var url = $"{_configuration.BaseUrl}/fchg/{evaNo}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var xmlText = await response.Content.ReadAsStringAsync();
        var rawTimetable = TimetableXmlParser.ParseXml(xmlText);

        return TimetableTransformer.TransformTimetable(rawTimetable);
    }

    /// <summary>
    /// Fetches recent changes (last 2 minutes) for a station
    /// </summary>
    /// <param name="evaNo">EVA station number</param>
    public async Task<Timetable> GetRecentChangesAsync(string evaNo)
    {
        var token = await GetAccessTokenAsync();
        var url = $"{_configuration.BaseUrl}/rchg/{evaNo}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var xmlText = await response.Content.ReadAsStringAsync();
        var rawTimetable = TimetableXmlParser.ParseXml(xmlText);

        return TimetableTransformer.TransformTimetable(rawTimetable);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}