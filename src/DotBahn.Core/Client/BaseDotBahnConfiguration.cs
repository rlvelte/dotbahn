namespace DotBahn.Core.Client;

/// <summary>
/// Configuration for DotBahn API
/// </summary>
public record BaseDotBahnConfiguration {
    /// <summary>
    /// The base url of the endpoint
    /// </summary>
    public required string BaseUrl { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}