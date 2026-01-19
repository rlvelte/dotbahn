using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Clients.Shared.Options;

/// <summary>
/// Options for a client.
/// </summary>
[ExcludeFromCodeCoverage]
public record ClientOptions {
    /// <summary>
    /// The base URI client.
    /// </summary>
    public required Uri BaseEndpoint { get; set; }
}
