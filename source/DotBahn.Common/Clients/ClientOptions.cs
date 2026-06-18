using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Common.Clients;

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
