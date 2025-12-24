namespace DotDB.Models;

/// <summary>
/// Managed platform information
/// </summary>
public class PlatformInfo
{
    public string Planned { get; set; }       // Planned platform
    public string Changed { get; set; }       // Changed platform
    public bool HasChanged { get; set; }      // Quick check if platform changed
}