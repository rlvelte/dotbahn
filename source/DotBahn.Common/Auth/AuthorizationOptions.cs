using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Common.Auth;

/// <summary>
/// Options for authorization.
/// </summary>
[ExcludeFromCodeCoverage]
public record AuthorizationOptions {
    /// <summary>
    /// The client ID for your application.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// The api key for your application.
    /// </summary>
    public required string ApiKey { get; set; }

    /// <inheritdoc />
    public sealed override string ToString() => $"ClientId: {(ClientId.IsWhiteSpace() ? "not set" : "set")}, ApiKey: {(ApiKey.IsWhiteSpace() ? "not set" : "set")}";
}
