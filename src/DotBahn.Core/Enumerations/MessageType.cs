namespace DotBahn.Core.Enumerations;

/// <summary>
/// Defines the type of a message or notification.
/// </summary>
public enum MessageType {
    /// <summary>
    /// Information about the quality of service.
    /// </summary>
    Quality,

    /// <summary>
    /// Information specifically about delays.
    /// </summary>
    Delay,

    /// <summary>
    /// General information message.
    /// </summary>
    Info,

    /// <summary>
    /// Information about a disruption in service.
    /// </summary>
    Disruption,

    /// <summary>
    /// Information about the cause of a delay.
    /// </summary>
    CauseOfDelay,

    /// <summary>
    /// A free-text message.
    /// </summary>
    FreeText,

    /// <summary>
    /// Information about connecting trains.
    /// </summary>
    Connection,

    /// <summary>
    /// Specific message type for Himmel (Sky) notifications.
    /// </summary>
    Himmel
}