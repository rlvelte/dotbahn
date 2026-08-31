using System.Text.Json;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Common.Parsing;

/// <summary>
/// Generic JSON parser implementation
/// </summary>
/// <typeparam name="TContract">The raw type to deserialize into</typeparam>
public class JsonParser<TContract> : IParser<TContract> where TContract : new() {
    private readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true,
        Converters = {
            new BahnDialectJsonConverter()
        }
    };

    /// <inheritdoc />
    /// <exception cref="JsonException">Thrown when deserialization fails or returns <c>null</c></exception>
    public TContract Parse(string input) {
        if (string.IsNullOrWhiteSpace(input)) {
            return new TContract();
        }

        return JsonSerializer.Deserialize<TContract>(input, _options) ?? throw new JsonException("Deserialization of API response returned null.");
    }
}
