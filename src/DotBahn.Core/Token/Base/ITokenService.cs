namespace DotBahn.Core.Token.Base;

/// <summary>
/// Service for managing OAuth access tokens for the Deutsche Bahn API
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gets a valid access token, refreshing it if necessary
    /// </summary>
    Task<string> GetAccessTokenAsync();
}