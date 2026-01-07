namespace DotBahn.Modules.Parsing.Base;

/// <summary>
/// Generic interface for parsing raw data into a target type.
/// </summary>
/// <typeparam name="TContract">The type to parse into.</typeparam>
public interface IParser<out TContract> {
    /// <summary>
    /// Parses the provided input string into the target type.
    /// </summary>
    /// <param name="input">The input data (e.g., XML, JSON).</param>
    /// <returns>The parsed object.</returns>
    TContract Parse(string input);
}
