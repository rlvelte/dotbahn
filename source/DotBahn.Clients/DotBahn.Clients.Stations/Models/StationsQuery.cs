using System.ComponentModel;
using DotBahn.Clients.Shared.Models;
using DotBahn.Clients.Stations.Enumerations;
using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Clients.Stations.Models;

/// <summary>
/// Represents the query parameters for searching stations in the Deutsche Bahn StaDa API.
/// Provides fluent methods for convenient building of queries.
/// </summary>
public sealed record StationsQuery {
    /// <summary>
    /// Strings to search for station names.
    /// The wildcards '*' (indicating an arbitrary number of characters) and '?' (indicating one single character) can be used in the search pattern.
    /// </summary>
    public string[]? Names { get;
        set {
            if (value == null || value.Length == 0) {
                throw new ArgumentException("At least one name is required.", nameof(value));
            }

            field = value.Select(n => n.Contains('*') || n.Contains('?') ? n : n + "*").ToArray();
        } 
    }

    /// <summary>
    /// Filter by station category.
    /// The category must be between 1 and 7, otherwise a parameter exception is returned.
    /// </summary>
    public string? Categories { 
        get;
        set {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("At least one category must be specified.", nameof(value));
            }

            var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts) {
                if (part.Contains('-')) {
                    var rangeParts = part.Split('-', 2, StringSplitOptions.TrimEntries);
                    if (rangeParts.Length != 2 || 
                        !int.TryParse(rangeParts[0], out var start) || 
                        !int.TryParse(rangeParts[1], out var end)) {
                        throw new ArgumentException($"Invalid category range: {part}", nameof(value));
                    }

                    if (start < 1 || start > 7 || end < 1 || end > 7 || start > end) {
                        throw new ArgumentException($"Category range out of bounds: {part}", nameof(value));
                    }
                }
                else {
                    if (!int.TryParse(part, out var parsed) || parsed < 1 || parsed > 7) {
                        throw new ArgumentException($"Category must be between 1 and 7: {part}", nameof(value));
                    }
                }
            }
        
            field = string.Join(',', parts);
        }
    }

    /// <summary>
    /// Filter by German federal state.
    /// </summary>
    public FederalState? State { get; set; }

    /// <summary>
    /// The EVA station number used as a unique identifier.
    /// </summary>
    public string? Eva { get; set; }

    /// <summary>
    /// The RIL100 identifier of the station.
    /// </summary>
    public string? Ril { get; set; }

    /// <summary>
    /// Logical operator for combining multiple filters.
    /// </summary>
    public LogicalOperator? Operator { get; set; } = LogicalOperator.And;

    /// <summary>
    /// Offset of the first hit returned in the QueryResult object with respect to all hits returned by the query.
    /// If this parameter is omitted, it will be set to 0 internally.
    /// </summary>
    public int? Offset { get; set; } = 0;

    /// <summary>
    /// The maximum number of hits to be returned by that query.
    /// If 'limit' is set greater than 10000, it will be reset to 10000 internally and only 10000 hits will be returned.
    /// </summary>
    public int? Limit { get; set; } = 10000;

    /// <summary>
    /// Sets the station names or fragments to search for.
    /// Appends a trailing '*' automatically if no wildcard is present.
    /// </summary>
    /// <param name="names"></param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the name is null or empty.</exception>
    public StationsQuery WithNames(params string[] names) {
        Names = names;
        return this;
    }

    /// <summary>
    /// Sets the station category filter.
    /// You can specify a single category, a range (e.g., "2-4") or a list of categories (e.g., "1,3-5").
    /// Categories must be between 1 and 7; otherwise, an <see cref="ArgumentException"/> is thrown.
    /// </summary>
    /// <param name="categories">One or more category specifications: integers, ranges, or comma-separated values.</param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if any category is invalid or out of range.</exception>
    public StationsQuery WithCategories(params string[] categories) {
        Categories = string.Join(',', categories);
        return this;
    }


    /// <summary>
    /// Filters stations by federal state.
    /// </summary>
    /// <param name="state">German federal state.</param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    public StationsQuery InFederalState(FederalState state) {
        State = state;
        return this;
    }

    /// <summary>
    /// Filters stations by EVA number.
    /// </summary>
    /// <param name="eva">EVA station number.</param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    public StationsQuery WithEva(string eva) {
        Eva = eva;
        return this;
    }

    /// <summary>
    /// Filters stations by RIL100 identifier.
    /// </summary>
    /// <param name="ril">RIL100 identifier.</param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    public StationsQuery WithRil(string ril) {
        Ril = ril;
        return this;
    }

    /// <summary>
    /// Sets the logical operator for combining multiple filters.
    /// </summary>
    /// <param name="operator">Logical operator.</param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    public StationsQuery CombineAs(LogicalOperator @operator) {
        Operator = @operator;
        return this;
    }

    /// <summary>
    /// Sets the number of results to skip for pagination.
    /// </summary>
    /// <param name="offset">Number of results to skip.</param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    public StationsQuery Skip(int offset) {
        Offset = offset;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of results to return.
    /// </summary>
    /// <param name="limit">Maximum results.</param>
    /// <returns>The current <see cref="StationsQuery"/> instance for fluent chaining.</returns>
    public StationsQuery LimitTo(int limit) {
        Limit = limit;
        return this;
    }
    
    /// <summary>
    /// Converts the query into <see cref="QueryParameters"/> for API calls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public QueryParameters ToQueryParameters() => QueryParameters.Create()
                                                                .Add("searchstring", Names != null ? string.Join(',', Names) : string.Empty)
                                                                .Add("category", Categories)
                                                                .Add("federalstate", State?.GetAssociatedValue())
                                                                .Add("eva", Eva)
                                                                .Add("ril", Ril)
                                                                .Add("logicaloperator", Operator?.GetAssociatedValue())
                                                                .Add("offset", Offset.ToString())
                                                                .Add("limit", Limit.ToString());
}