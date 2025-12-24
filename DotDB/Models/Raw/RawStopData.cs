namespace DotDB.Models.Raw;

/// <summary>
/// Raw XML structure for a single stop
/// </summary>
public record RawStopData : RawStop
{
    public RawTripLabel Tl { get; set; }        // Trip label (train information)
    public RawArrival Ar { get; set; }          // Arrival information
    public RawDeparture Dp { get; set; }        // Departure information
    public List<RawMessage> M { get; set; }     // Messages/disruptions
}