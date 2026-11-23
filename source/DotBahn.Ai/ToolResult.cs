namespace DotBahn.Ai;

/// <summary>
/// Structured error information returned to the model when a tool call fails.
/// </summary>
/// <param name="Code">Stable machine-readable error code</param>
/// <param name="Message">Sanitized, actionable error message</param>
/// <param name="Field">Optional name of the invalid input field</param>
/// <param name="Retryable">Whether retrying with corrected input may succeed</param>
public sealed record ToolError(string Code, string Message, string? Field = null, bool Retryable = false);

/// <summary>
/// Result envelope returned by AI tools, containing either data or a structured error.
/// </summary>
/// <typeparam name="T">The type of the successful result</typeparam>
/// <param name="Success">Whether the tool completed successfully</param>
/// <param name="Data">The successful result, or <c>null</c> on failure</param>
/// <param name="Error">The structured error, or <c>null</c> on success</param>
public sealed record ToolResult<T>(bool Success, T? Data, ToolError? Error) {
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="data">The successful result</param>
    /// <returns>A successful <see cref="ToolResult{T}"/></returns>
    public static ToolResult<T> Ok(T data) => new(true, data, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The structured error</param>
    /// <returns>A failed <see cref="ToolResult{T}"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <c>null</c></exception>
    public static ToolResult<T> Fail(ToolError error) {
        ArgumentNullException.ThrowIfNull(error);
        return new ToolResult<T>(false, default, error);
    }
}
