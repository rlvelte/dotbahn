using System.Text.Json.Serialization;

namespace DotBahn.Timetables.Models.Enumerations;

/// <summary>
/// Status of an arrival or departure event.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<EventStatus>))]
public enum EventStatus {
    /// <summary>
    /// The event was planned. This status is also used when the cancellation of an event has been revoked.
    /// </summary>
    [JsonStringEnumMemberName("p")]
    Planned,

    /// <summary>
    /// The event was added to the planned data (new stop).
    /// </summary>
    [JsonStringEnumMemberName("a")]
    Added,

    /// <summary>
    /// The event was canceled. As a changed status, this can apply to both planned and added stops.
    /// </summary>
    [JsonStringEnumMemberName("c")]
    Cancelled,

    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
