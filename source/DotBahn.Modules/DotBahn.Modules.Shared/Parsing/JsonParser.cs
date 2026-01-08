using System.Text.Json;

namespace DotBahn.Modules.Shared.Parsing;

/// <summary>
/// Generic JSON parser implementation.
/// </summary>
/// <typeparam name="TContract">The raw type to deserialize into.</typeparam>
public class JsonParser<TContract> : IParser<TContract> {
    private static readonly JsonSerializerOptions Options = new() {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc />
    public TContract Parse(string input) {
        return JsonSerializer.Deserialize<TContract>(input, Options)!;
    }
}
