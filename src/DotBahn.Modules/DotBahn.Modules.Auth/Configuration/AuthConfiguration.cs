namespace DotBahn.Modules.Shared;

public abstract record AuthConfiguration {
    /// <summary>
    /// The Client ID for authentication.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Client Secret for authentication.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}