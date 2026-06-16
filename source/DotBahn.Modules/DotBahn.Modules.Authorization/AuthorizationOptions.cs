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

    /// <summary>
    /// The HTTP header name used for the client ID.
    /// </summary>
    public string HeaderNameClientId { get; set; } = "DB-Client-Id";

    /// <summary>
    /// The HTTP header name used for the API key.
    /// </summary>
    public string HeaderNameApiKey { get; set; } = "DB-Api-Key";

    /// <inheritdoc />
    public sealed override string ToString() => $"ClientId: {(ClientId.IsWhiteSpace() ? "not set" : "set")}, ApiKey: {(ApiKey.IsWhiteSpace() ? "not set" : "set")}";
}
