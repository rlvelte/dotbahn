using System.Text.Json.Serialization;

namespace DotBahn.Data.Timetables.Enumerations;

/// <summary>
/// Type of trip/train service.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<TripType>))]
public enum TripType {
    /// <summary>
    /// Regular passenger service.
    /// </summary>
    [JsonStringEnumMemberName("p")]
    Passenger,

    /// <summary>
    /// Empty train movement (no passengers).
    /// </summary>
    [JsonStringEnumMemberName("e")]
    Empty,

    /// <summary>
    /// Additional train type (z).
    /// </summary>
    [JsonStringEnumMemberName("z")]
    Z,

    /// <summary>
    /// Additional train type (s).
    /// </summary>
    [JsonStringEnumMemberName("s")]
    S,

    /// <summary>
    /// Additional train type (h).
    /// </summary>
    [JsonStringEnumMemberName("h")]
    H,

    /// <summary>
    /// Additional train type (n).
    /// </summary>
    [JsonStringEnumMemberName("n")]
    N,

    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
