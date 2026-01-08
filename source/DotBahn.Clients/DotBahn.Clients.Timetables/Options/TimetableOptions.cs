using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Timetables.Options;

/// <summary>
/// Options for the DotBahn API clients.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record TimetableOptions {
    /// <summary>
    /// The base URI of the API.
    /// </summary>
    public required Uri BaseEndpoint { get; set; } = new("https://api.deutschebahn.com/timetables/v1");
}