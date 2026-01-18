using System.Text.Json;
using DotBahn.Modules.Shared.Parsing.Additional;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Modules.Shared.Parsing;

/// <summary>
/// Generic JSON parser implementation.
/// </summary>
/// <typeparam name="TContract">The raw type to deserialize into.</typeparam>
public class JsonParser<TContract> : IParser<TContract> where TContract : new() {
    private readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true,
        Converters = {
            new BahnDialectJsonConverter()
        }
    };

    /// <inheritdoc />
    public TContract Parse(string input) => 
        string.IsNullOrWhiteSpace(input) ? new TContract() : JsonSerializer.Deserialize<TContract>(input, _options)!;
}
