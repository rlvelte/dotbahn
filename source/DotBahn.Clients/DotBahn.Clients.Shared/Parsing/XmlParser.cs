using System.Xml;
using System.Xml.Serialization;

using DotBahn.Clients.Shared.Parsing.Base;

namespace DotBahn.Clients.Shared.Parsing;

/// <summary>
/// Generic XML parser implementation.
/// </summary>
/// <typeparam name="TContract">The raw type to deserialize into.</typeparam>
public class XmlParser<TContract> : IParser<TContract> where TContract : new() {
    private readonly XmlSerializer _serializer = new(typeof(TContract));
    private readonly XmlReaderSettings _settings = new() {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null
    };

    /// <inheritdoc />
    public TContract Parse(string input) {
        if (string.IsNullOrWhiteSpace(input)) {
            return new TContract();
        }

        using var stringReader = new StringReader(input);
        using var xmlReader = XmlReader.Create(stringReader, _settings);

        return (TContract)_serializer.Deserialize(xmlReader)!;
    }
}
