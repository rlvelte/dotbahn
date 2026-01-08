using System.Diagnostics.CodeAnalysis;
using DotBahn.Modules.Authorization.Enumerations;
using JetBrains.Annotations;

namespace DotBahn.Modules.Authorization;

/// <summary>
/// Configuration options for the authorization system
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record ModuleOptions {
    /// <summary>
    /// The type of authorization provider to use
    /// </summary>
    public AuthProviderType ProviderType { get; set; } = AuthProviderType.ApiKey;

    /// <summary>
    /// The Client ID for authentication.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Client Secret for authentication.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}