namespace DotDB.Models;

/// <summary>
/// Managed time information with delay calculation
/// </summary>
public class TimeInfo
{
    public DateTime Planned { get; set; }     // Planned time
    public DateTime? Changed { get; set; }    // Changed/actual time
    public int? Delay { get; set; }           // Delay in minutes (calculated)
    public bool IsDelayed { get; set; }       // Quick check if delayed
}