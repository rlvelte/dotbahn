namespace DotBahn.Clients.Shared.Models;

/// <summary>
/// Helper class for building query string parameters in a fluent way.
/// </summary>
public class QueryParameters {
    private readonly Dictionary<string, string> _parameters = new();

    /// <summary>
    /// Creates a new instance of <see cref="QueryParameters"/>.
    /// </summary>
    /// <returns>A new <see cref="QueryParameters"/> instance.</returns>
    public static QueryParameters Create() => new();

    /// <summary>
    /// Checks if any query parameters have been added.
    /// </summary>
    /// <returns>True if there is at least one parameter; otherwise, false.</returns>
    public bool Any() => _parameters.Count > 0;

    /// <summary>
    /// Adds a key-value pair to the query parameters if the value is not null, empty, or whitespace.
    /// </summary>
    /// <param name="key">The query parameter key.</param>
    /// <param name="value">The query parameter value.</param>
    /// <returns>The current <see cref="QueryParameters"/> instance for fluent chaining.</returns>
    public QueryParameters Add(string key, string? value) {
        if (!string.IsNullOrWhiteSpace(value)) {
            _parameters[key] = value;
        }
        return this;
    }

    /// <summary>
    /// Adds a key with multiple values to the query parameters as a comma-separated list.
    /// </summary>
    /// <typeparam name="T">Type of the values.</typeparam>
    /// <param name="key">The query parameter key.</param>
    /// <param name="values">The collection of values to add.</param>
    /// <returns>The current <see cref="QueryParameters"/> instance for fluent chaining.</returns>
    public QueryParameters Add<T>(string key, IEnumerable<T>? values) {
        if (values == null) {
            return this;
        }

        var list = values.ToList();
        if (list.Count > 0) {
            _parameters[key] = string.Join(",", list);
        }
        return this;
    }

    /// <summary>
    /// Converts the added parameters into a URL-encoded query string.
    /// </summary>
    /// <returns>A URL-encoded query string (e.g., "key1=value1&amp;key2=value2") or an empty string if no parameters exist.</returns>
    public string ToQueryString() =>
        _parameters.Count == 0 ? string.Empty : string.Join("&", _parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
}
