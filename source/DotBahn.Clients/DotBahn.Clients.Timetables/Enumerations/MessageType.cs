using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Clients.Timetables.Enumerations;

/// <summary>
/// Defines the type of message or notification.
/// </summary>
public enum MessageType {
    /// <summary>
    /// Information about the quality of service.
    /// </summary>
    [AssociatedValue("q")]
    Quality,

    /// <summary>
    /// Information specifically about delays.
    /// </summary>
    [AssociatedValue("d")]
    Delay,

    /// <summary>
    /// General information message.
    /// </summary>
    [AssociatedValue("i")]
    Info,

    /// <summary>
    /// Information about a disruption in service.
    /// </summary>
    [AssociatedValue("r")]
    Disruption,

    /// <summary>
    /// Information about the cause of a delay.
    /// </summary>
    [AssociatedValue("u")]
    CauseOfDelay,

    /// <summary>
    /// A free-text message.
    /// </summary>
    [AssociatedValue("f")]
    FreeText,

    /// <summary>
    /// Information about connecting trains.
    /// </summary>
    [AssociatedValue("c")]
    Connection,

    /// <summary>
    /// Specific message type for Himmel (Sky) notifications.
    /// </summary>
    [AssociatedValue("h")]
    Himmel
}