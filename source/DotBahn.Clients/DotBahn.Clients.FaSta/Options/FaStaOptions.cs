using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Clients.FaSta.Options;

/// <summary>
/// Options for the FaSta API client.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record FaStaOptions {
    /// <summary>
    /// The base URI of the FaSta API.
    /// </summary>
    public required Uri BaseEndpoint { get; set; } = new("https://api.deutschebahn.com/fasta/v2/");
}
