namespace DotDB.Models.Additional;

/// <summary>
/// An OAuth token response
/// </summary>
public record OAuthToken {
    public required string AccessToken { get; set; }
    public required string TokenType { get; set; }
    public int ExpiresIn { get; set; }
}