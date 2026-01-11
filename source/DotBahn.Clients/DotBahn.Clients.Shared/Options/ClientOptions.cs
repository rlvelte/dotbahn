using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Clients.Shared.Options;

/// <summary>
/// Options for a client.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record ClientOptions {
    /// <summary>
    /// The base URI client.
    /// </summary>
    public required Uri BaseEndpoint { get; set; }
}
