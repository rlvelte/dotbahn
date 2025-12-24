namespace DotDB.Models;

/// <summary>
/// Distant endpoint information
/// </summary>
public record DistantEndpoint {
    public string Planned { get; set; }
    public string Changed { get; set; }
}