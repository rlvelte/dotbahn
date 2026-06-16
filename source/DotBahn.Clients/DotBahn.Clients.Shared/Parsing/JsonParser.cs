using System.Text.Json;

using DotBahn.Clients.Shared.Parsing.Additional;
using DotBahn.Clients.Shared.Parsing.Base;

namespace DotBahn.Clients.Shared.Parsing;

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
    public TContract Parse(string input) => string.IsNullOrWhiteSpace(input) ? new TContract() : JsonSerializer.Deserialize<TContract>(input, _options)!;
}
