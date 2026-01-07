using System.Text.Json.Serialization;

namespace DotBahn.Core.Token.Additional;

/// <summary>
/// OAuth token response from Deutsche Bahn API
/// </summary>
public record OAuthToken
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}