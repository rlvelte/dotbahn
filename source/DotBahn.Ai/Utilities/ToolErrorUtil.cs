using System.Net;

namespace DotBahn.Ai.Utilities;

/// <summary>
/// Maps exceptions to sanitized, model-facing <see cref="ToolError"/> instances.
/// </summary>
public static class ToolErrorUtil {
    /// <summary>
    /// Maps an exception to a structured tool error.
    /// </summary>
    /// <param name="exception">The exception to map</param>
    /// <returns>The structured tool error</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception"/> is <c>null</c></exception>
    public static ToolError FromException(Exception exception) {
        ArgumentNullException.ThrowIfNull(exception);
        return exception switch {
            ArgumentException argument => FromArgument(argument),
            HttpRequestException http => FromHttp(http),
            _ => new ToolError("internal_error", "The tool could not complete the request.")
        };
    }

    private static ToolError FromHttp(HttpRequestException http) => http.StatusCode switch {
        HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new ToolError("unauthorized", "DotBahn API credentials are missing or invalid.", null, false),
        HttpStatusCode.NotFound => new ToolError("not_found", "The requested station or board was not found.", null, false),
        HttpStatusCode.TooManyRequests => new ToolError("rate_limited", "The DotBahn API rate limit was reached. Retry later.", null, true),
        { } value when (int)value >= 500 => new ToolError("upstream_error", "The DotBahn API is temporarily unavailable.", null, true),
        _ => new ToolError("upstream_error", "The DotBahn API request failed.", null, true)
    };

    private static ToolError FromArgument(ArgumentException argument) => new("invalid_argument", argument.Message, string.IsNullOrWhiteSpace(argument.ParamName) ? null : argument.ParamName, true);
}
