using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace DotBahn.Common.Parsing;

/// <summary>
/// Generic JSON parser implementation using source-generated metadata
/// </summary>
/// <typeparam name="TContract">The raw type to deserialize into</typeparam>
/// <param name="type">The source-generated JSON type info for <typeparamref name="TContract"/></param>
public class JsonParser<TContract>(JsonTypeInfo<TContract> type) : IParser<TContract> where TContract : new() {
    /// <inheritdoc />
    /// <exception cref="JsonException">Thrown when deserialization fails or returns <c>null</c></exception>
    public TContract Parse(string input) {
        if (string.IsNullOrWhiteSpace(input)) {
            return new TContract();
        }

        return JsonSerializer.Deserialize(input, type) ?? throw new JsonException("Deserialization of API response returned null");
    }
}
