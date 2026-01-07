using DotBahn.Modules.Auth.Configuration;
using DotBahn.Modules.Cache.Configuration;

namespace DotBahn.TimetableApi.Configuration;

/// <summary>
/// Options for the DotBahn API clients.
/// </summary>
public record TimetableConfiguration {
    /// <summary>
    /// The base URI of the API.
    /// </summary>
    public required Uri BaseEndpoint { get; set; } = new("https://api.deutschebahn.com/timetables/v1");
    
    /// <summary>
    /// Configuration for the authorization provider.
    /// </summary>
    public required AuthConfiguration Auth { get; set; }
    
    /// <summary>
    /// Configuration for the cache provider.
    /// </summary>
    public CacheConfiguration? Cache { get; set; }
}