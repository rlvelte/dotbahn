using System.Diagnostics.CodeAnalysis;
using DotBahn.Modules.Auth.Enumerations;
using JetBrains.Annotations;

namespace DotBahn.Modules.Auth.Configuration;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record AuthConfiguration {
    /// <summary>
    /// The type of authorization provider to use
    /// </summary>
    public AuthProviderType ProviderType { get; set; } = AuthProviderType.Token;

    /// <summary>
    /// The Client ID for authentication.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Client Secret for authentication.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}