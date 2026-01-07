using System.Xml.Serialization;

namespace DotBahn.Modules.Shared.Parsing;

/// <summary>
/// Generic XML parser implementation.
/// </summary>
/// <typeparam name="TContract">The raw type to deserialize into.</typeparam>
public class XmlParser<TContract> : IParser<TContract> {
    private readonly XmlSerializer _serializer = new(typeof(TContract));

    /// <inheritdoc />
    public TContract Parse(string input) {
        using var reader = new StringReader(input);
        return (TContract)_serializer.Deserialize(reader)!;
    }
}
