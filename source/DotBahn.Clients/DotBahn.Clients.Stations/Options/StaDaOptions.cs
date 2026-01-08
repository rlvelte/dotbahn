using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Clients.Stations.Options;

/// <summary>
/// Options for the DotBahn API clients.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record StaDaOptions {
    /// <summary>
    /// The base URI of the API.
    /// </summary>
    public required Uri BaseEndpoint { get; set; } = new("https://api.deutschebahn.com/stada/v2/");
}