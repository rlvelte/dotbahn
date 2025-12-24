namespace DotDB.Models;

/// <summary>
/// Managed path information (route)
/// </summary>
public class PathInfo
{
    public List<string> Planned { get; set; }      // Planned stops (station names)
    public List<string> Changed { get; set; }      // Changed stops
    public bool HasChanged { get; set; }           // Quick check if route changed
}