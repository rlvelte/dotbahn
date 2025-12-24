using DotDB.Models.Enumerations;

namespace DotDB.Models;

/// <summary>
/// Managed train information
/// </summary>
public class TrainInfo
{
    public string Category { get; set; }      // Train category (ICE, IC, RE, S, etc.)
    public string Number { get; set; }        // Train number
    public TripType Type { get; set; }        // Trip type
    public string Owner { get; set; }         // Operator code
    public string FilterFlags { get; set; }   // Filter flags
}