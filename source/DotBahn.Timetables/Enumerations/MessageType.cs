using System.Text.Json.Serialization;

namespace DotBahn.Timetables.Enumerations;

/// <summary>
/// Type of timetable message.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MessageType>))]
public enum MessageType {
    /// <summary>
    /// A HIM message.
    /// </summary>
    [JsonStringEnumMemberName("h")]
    Him,

    /// <summary>
    /// A message about a quality change.
    /// </summary>
    [JsonStringEnumMemberName("q")]
    QualityChange,

    /// <summary>
    /// A free text message.
    /// </summary>
    [JsonStringEnumMemberName("f")]
    Free,

    /// <summary>
    /// A message about the cause of a delay.
    /// </summary>
    [JsonStringEnumMemberName("d")]
    CauseOfDelay,

    /// <summary>
    /// An IBIS message (generated from IRIS-AP).
    /// </summary>
    [JsonStringEnumMemberName("i")]
    Ibis,

    /// <summary>
    /// An IBIS message (generated from IRIS-AP) not yet assigned to a train.
    /// </summary>
    [JsonStringEnumMemberName("u")]
    UnassignedIbis,

    /// <summary>
    /// A major disruption.
    /// </summary>
    [JsonStringEnumMemberName("r")]
    Disruption,

    /// <summary>
    /// A connection message.
    /// </summary>
    [JsonStringEnumMemberName("c")]
    Connection,

    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
