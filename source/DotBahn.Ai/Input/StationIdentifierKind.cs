namespace DotBahn.Ai.Input;

/// <summary>
/// Identifier scheme used by <see cref="BoardToolInput.StationId"/>.
/// </summary>
public enum StationIdentifierKind {
    /// <summary>
    /// EVA station number.
    /// </summary>
    Eva,

    /// <summary>
    /// STADA station identifier.
    /// </summary>
    Stada,

    /// <summary>
    /// DHID stop-place identifier.
    /// </summary>
    Dhid
}
