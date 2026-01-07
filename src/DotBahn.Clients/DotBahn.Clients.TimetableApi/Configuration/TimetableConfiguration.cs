using DotBahn.Modules.Auth.Configuration;
using DotBahn.Modules.Cache.Configuration;
using DotBahn.Modules.Shared;

namespace DotBahn.TimetableApi.Configuration;

/// <summary>
/// Options for the DotBahn API clients.
/// </summary>
public record TimetableConfiguration {
    /// <summary>
    /// The base URI of the API.
    /// </summary>
    public Uri BaseEndpoint { get; set; } = new("https://api.deutschebahn.com/timetables/v1");
    
    public AuthConfiguration Auth { get; set; }
    public CacheConfiguration Cache { get; set; }
}