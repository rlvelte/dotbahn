namespace DotBahn.Boards.Models.Messages;

/// <summary>
/// Type of board message.
/// </summary>
public enum MessageType {
    /// <summary>
    /// Journey attribute.
    /// </summary>
    Attribute,

    /// <summary>
    /// Disruption communication.
    /// </summary>
    Disruption,

    /// <summary>
    /// General note.
    /// </summary>
    Note,

    /// <summary>
    /// RIS cause code.
    /// </summary>
    RisCauseCode,

    /// <summary>
    /// RIS quality deviation.
    /// </summary>
    RisQualityDeviation
}
