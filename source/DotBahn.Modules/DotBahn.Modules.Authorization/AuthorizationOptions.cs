using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Modules.Authorization;

/// <summary>
/// Configuration options for the authorization system
/// </summary>
[ExcludeFromCodeCoverage]
public record AuthorizationOptions {
    /// <summary>
    /// The Client ID for authentication.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// The API key for authentication.
    /// </summary>
    public required string ApiKey { get; set; }
}