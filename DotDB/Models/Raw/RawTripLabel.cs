namespace DotDB.Models.Raw;

/// <summary>
/// Raw attributes for trip label (train information)
/// </summary>
public record RawTripLabel {
    public string F { get; init; }  // Filter flags (e.g., "F" for Fernverkehr/long-distance)
    public string T { get; init; }  // Trip type (e.g., "p" for passenger)
    public string O { get; init; }  // Owner/operator (e.g., "80" for DB)
    public string C { get; init; }  // Category (e.g., "ICE", "IC", "RE")
    public string N { get; init; }  // Train number
}