namespace DotBahn.Clients.Shared.Queries;

public class QueryParameters {
    private readonly Dictionary<string, string> _parameters = new();

    public static QueryParameters Create() => new();

    public bool Any() => _parameters.Count > 0;

    public QueryParameters Add(string key, string? value) {
        if (!string.IsNullOrWhiteSpace(value)) {
            _parameters[key] = value;
        }
        
        return this;
    }

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
    /// Converts the parameters to a query string.
    /// </summary>
    public string ToQueryString() =>
        _parameters.Count == 0 ? string.Empty : string.Join("&", _parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

}