using System.Text.Json.Serialization;

namespace DotBahn.Facilities.Models.Enumerations;

/// <summary>
/// Represents the type of facility in a station.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<FacilityType>))]
public enum FacilityType {
    /// <summary>
    /// Elevator facility.
    /// </summary>
    [JsonStringEnumMemberName("ELEVATOR")]
    Elevator,

    /// <summary>
    /// Escalator facility.
    /// </summary>
    [JsonStringEnumMemberName("ESCALATOR")]
    Escalator,

    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
