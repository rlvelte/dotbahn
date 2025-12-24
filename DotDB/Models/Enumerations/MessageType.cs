namespace DotDB.Models.Enumerations;

/// <summary>
/// Message type enum
/// </summary>
public enum MessageType {
    Quality,         // q - Quality message
    Delay,           // d - Delay message
    Info,            // i - Information message
    Disruption,      // r - Disruption
    CauseOfDelay,    // u - Cause of delay
    FreeText,        // f - Free text message
    Connection,      // c - Connection information
    Himmel          // h - Himmel message (quality/sky message)
}