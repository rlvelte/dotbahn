namespace DotBahn.Common.Clients;

/// <summary>
/// Options for a client
/// </summary>
public record ClientOptions {
    /// <summary>
    /// The base endpoint for API requests
    /// </summary>
    public required Uri BaseEndpoint { get; set; }
}
