using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Clients.Shared.Options;

/// <summary>
/// Options for a client.
/// </summary>
[ExcludeFromCodeCoverage]
public record ClientOptions {
    /// <summary>
    /// The base endpoint for API requests.
    /// </summary>
    public required Uri BaseEndpoint { get; set; }

    /// <summary>
    /// The Client ID for authentication. If not set, uses shared authorization.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// The API key for authentication. If not set, uses shared authorization.
    /// </summary>
    public string? ApiKey { get; set; }
}
