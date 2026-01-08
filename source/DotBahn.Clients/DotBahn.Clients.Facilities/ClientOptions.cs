using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Clients.Facilities;

/// <summary>
/// Options for the FaSta API client.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record ClientOptions {
    /// <summary>
    /// The base URI of the FaSta API.
    /// </summary>
    public required Uri BaseEndpoint { get; set; } = new("https://api.deutschebahn.com/fasta/v2/");
}
