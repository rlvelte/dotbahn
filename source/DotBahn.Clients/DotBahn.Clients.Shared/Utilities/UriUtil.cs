using System.Runtime.CompilerServices;

using DotBahn.Clients.Shared.Query;

namespace DotBahn.Clients.Shared.Utilities;

/// <summary>
/// Helper class for building URLs with query parameters.
/// </summary>
internal static class UriUtil {
    /// <summary>
    /// Builds a URL with query parameters.
    /// </summary>
    /// <param name="relative">The base relative URL.</param>
    /// <param name="queryParams">Optional query parameters.</param>
    /// <returns>The complete URL with a query string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string BuildUrl(string relative, QueryParameters? queryParams) {
        if (queryParams == null || !queryParams.Any()) {
            return relative;
        }

        var queryString = queryParams.ToQueryString();
        return string.IsNullOrEmpty(queryString) ? relative : $"{relative}?{queryString}";
    }
}
