namespace DotBahn.Clients.Shared.Base;

/// <summary>
/// Fluent builder for query parameters.
/// </summary>
public class QueryParameters {
    private readonly Dictionary<string, string> _parameters = new();

    /// <summary>
    /// Adds a string parameter if the value is not null or empty.
    /// </summary>
    public QueryParameters Add(string key, string? value) {
        if (!string.IsNullOrWhiteSpace(value)) {
            _parameters[key] = value;
        }
        
        return this;
    }

    /// <summary>
    /// Adds an integer parameter if the value is not null.
    /// </summary>
    public QueryParameters Add(string key, int? value) {
        if (value.HasValue) {
            _parameters[key] = value.Value.ToString();
        }
        
        return this;
    }

    /// <summary>
    /// Adds an array parameter if the array is not null or empty.
    /// </summary>
    public QueryParameters Add(string key, int[]? values) {
        if (values is { Length: > 0 }) {
            _parameters[key] = string.Join(",", values);
        }
        
        return this;
    }

    /// <summary>
    /// Adds an enumerable parameter if not null or empty.
    /// </summary>
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
    /// Adds a boolean parameter if the value is not null.
    /// </summary>
    public QueryParameters Add(string key, bool? value) {
        if (value.HasValue) {
            _parameters[key] = value.Value.ToString().ToLowerInvariant();
        }
        
        return this;
    }

    /// <summary>
    /// Checks if any parameters have been added.
    /// </summary>
    public bool Any() => _parameters.Count > 0;

    /// <summary>
    /// Converts the parameters to a query string.
    /// </summary>
    public string ToQueryString() => 
        _parameters.Count == 0 ? string.Empty : string.Join("&", _parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

    /// <summary>
    /// Creates a new instance of QueryParameters.
    /// </summary>
    public static QueryParameters Create() => new();
}