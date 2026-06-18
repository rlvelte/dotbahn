using System.Text.RegularExpressions;
using DotBahn.Common.Utilities;
using DotBahn.Stations.Models.Enumerations;
using StationsJsonContext = DotBahn.Stations.Internal.Json.StationsJsonContext;

namespace DotBahn.Stations;

/// <summary>
/// Represents the query parameters for searching stations.
/// Provides fluent methods for convenient building of queries.
/// </summary>
public sealed partial record StationQuery {
    /// <summary>
    /// Matches a category range pattern (e.g., "2-4", "1 - 7").
    /// </summary>
    [GeneratedRegex(@"^(\d+)\s*-\s*(\d+)$")]
    private static partial Regex ComplexCategoryRegex();

    /// <summary>
    /// Strings to search for station names.
    /// <remarks>
    /// Wildcards '*' and '?' are supported.
    /// </remarks>
    /// </summary>
    public string[]? Names { get; set; }

    /// <summary>
    /// Filter by station category.
    /// <remarks>
    /// Single values (1-7), ranges (e.g., "2-4"), or comma-separated (e.g., "1,3-5").
    /// </remarks>
    /// </summary>
    public string? Categories { get; set; }

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
    /// <remarks>
    /// Default is <see cref="LogicalOperator.And"/>.
    /// </remarks>
    /// </summary>
    public LogicalOperator Operator { get; set; } = LogicalOperator.And;

    /// <summary>
    /// Offset of the first hit returned.
    /// <remarks>
    /// Default is 0.
    /// </remarks>
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// The maximum number of hits to return.
    /// <remarks>
    /// Default is 10_000.
    /// </remarks>
    /// </summary>
    public int Limit { get; set; } = 10000;

    /// <summary>
    /// Sets the station names or fragments to search for.
    /// </summary>
    /// <remarks>
    /// Appends a trailing '*' automatically if no wildcard is present.
    /// </remarks>
    /// <param name="names">One or more station name patterns.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if no names are provided.</exception>
    public StationQuery WithNames(params string[] names) {
        if (names is not { Length: > 0 }) {
            throw new ArgumentException("At least one name is required.", nameof(names));
        }

        Names = [.. names.Select(n => n.Contains('*') || n.Contains('?') ? n : n + "*")];
        return this;
    }

    /// <summary>
    /// Sets the station category filter.
    /// <remarks>
    /// You can specify a single category, a range (e.g., "2-4") or a list of categories (e.g., "1,3-5"). Categories must be between 1 and 7.
    /// </remarks>
    /// </summary>
    /// <param name="categories">One or more category specifications: integers, ranges, or comma-separated values.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if any category is invalid or out of range (1-7).</exception>
    public StationQuery WithCategories(string categories) {
        if (string.IsNullOrWhiteSpace(categories)) {
            throw new ArgumentException("At least one category must be specified.", nameof(categories));
        }

        var parts = categories.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var normalized = new List<string>();

        foreach (var part in parts) {
            var trimmed = part.Trim();

            if (trimmed.Contains('-')) {
                var rangeMatch = ComplexCategoryRegex().Match(trimmed);
                if (!rangeMatch.Success) {
                    throw new ArgumentException($"Invalid category range: {part}", nameof(categories));
                }

                var start = int.Parse(rangeMatch.Groups[1].Value);
                var end = int.Parse(rangeMatch.Groups[2].Value);
                if (start < 1 || start > 7 || end < 1 || end > 7 || start > end) {
                    throw new ArgumentException($"Category range out of bounds: {part}", nameof(categories));
                }

                normalized.Add($"{start}-{end}");
            } else {
                if (!int.TryParse(trimmed, out var parsed) || parsed is < 1 or > 7) {
                    throw new ArgumentException($"Category must be between 1 and 7: {part}", nameof(categories));
                }

                normalized.Add(parsed.ToString());
            }
        }

        Categories = string.Join(',', normalized);
        return this;
    }

    /// <summary>
    /// Filters stations by federal state.
    /// </summary>
    /// <param name="state">German federal state.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    public StationQuery InFederalState(FederalState state) {
        State = state;
        return this;
    }

    /// <summary>
    /// Filters stations by EVA number.
    /// </summary>
    /// <param name="eva">EVA station number.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    public StationQuery WithEva(string eva) {
        Eva = eva;
        return this;
    }

    /// <summary>
    /// Filters stations by RIL100 identifier.
    /// </summary>
    /// <param name="ril">RIL100 identifier.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    public StationQuery WithRil(string ril) {
        Ril = ril;
        return this;
    }

    /// <summary>
    /// Sets the logical operator for combining multiple filters.
    /// </summary>
    /// <param name="operator">Logical operator.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    public StationQuery CombineAs(LogicalOperator @operator) {
        Operator = @operator;
        return this;
    }

    /// <summary>
    /// Sets the number of results to skip for pagination.
    /// </summary>
    /// <param name="offset">Number of results to skip.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    public StationQuery Skip(int offset) {
        Offset = offset;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of results to return.
    /// </summary>
    /// <param name="limit">Maximum results.</param>
    /// <returns>The current <see cref="StationQuery"/> instance for fluent chaining.</returns>
    public StationQuery LimitTo(int limit) {
        Limit = limit;
        return this;
    }

    /// <summary>
    /// Converts the query into <see cref="QueryParameters"/> for API calls.
    /// </summary>
    internal QueryParameters ToQueryParameters() => QueryParameters.Create()
        .Add("searchstring", Names != null ? string.Join(',', Names) : string.Empty)
        .Add("category", Categories)
        .Add("federalstate", EnumUtil.Format(State, StationsJsonContext.Default.FederalState))
        .Add("eva", Eva)
        .Add("ril", Ril)
        .Add("logicaloperator", EnumUtil.Format(Operator, StationsJsonContext.Default.LogicalOperator))
        .Add("offset", Offset.ToString())
        .Add("limit", Limit.ToString());
}
