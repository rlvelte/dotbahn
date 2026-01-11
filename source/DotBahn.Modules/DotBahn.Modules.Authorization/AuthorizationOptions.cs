using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Modules.Authorization;

/// <summary>
/// Configuration options for the authorization system
/// </summary>
[UsedImplicitly]
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