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
}
