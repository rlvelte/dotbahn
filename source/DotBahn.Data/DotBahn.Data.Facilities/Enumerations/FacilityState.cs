
using System.Text.Json.Serialization;

namespace DotBahn.Data.Facilities.Enumerations;

/// <summary>
/// Represents the operational state of a facility.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<FacilityState>))]
public enum FacilityState {
    /// <summary>
    /// Facility is active and operational.
    /// </summary>
    [JsonStringEnumMemberName("ACTIVE")]
    Active,

    /// <summary>
    /// Facility is inactive or out of service.
    /// </summary>
    [JsonStringEnumMemberName("INACTIVE")]
    Inactive,

    /// <summary>
    /// Facility state is unknown or not specified.
    /// </summary>
    [JsonStringEnumMemberName("UNKNOWN")]
    Unknown
}
