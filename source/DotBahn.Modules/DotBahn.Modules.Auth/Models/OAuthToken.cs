using System.Text.Json.Serialization;

namespace DotBahn.Modules.Auth.Models;

/// <summary>
/// OAuth token response from Deutsche Bahn API
/// </summary>
public partial record OAuthToken {
    /// <summary>
    /// The access token issued by the authorization server.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    /// <summary>
    /// The type of the token (typically "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    /// <summary>
    /// The lifetime in seconds of the access token.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}