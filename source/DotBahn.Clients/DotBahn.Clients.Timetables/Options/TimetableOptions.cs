using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DotBahn.Clients.Timetables.Options;

/// <summary>
/// Options for the DotBahn API clients.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public record TimetableOptions {
    /// <summary>
    /// The base URI of the API.
    /// </summary>
    public required Uri BaseEndpoint { get; set; } = new("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/");
}