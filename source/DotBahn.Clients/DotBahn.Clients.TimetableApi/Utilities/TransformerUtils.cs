using System.Runtime.CompilerServices;

namespace DotBahn.TimetableApi.Utilities;

/// <summary>
/// Utility methods for parsing API data during transformation.
/// </summary>
public static class TransformerUtils {
    /// <summary>
    /// Parses time string from API format (YYMMDDhhmm) to DateTime.
    /// </summary>
    /// <param name="timeStr">The time string from the API.</param>
    /// <returns>A DateTime representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the format is invalid.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ParseApiTime(string timeStr) {
        if (string.IsNullOrEmpty(timeStr) || timeStr.Length != 10 ||
            !int.TryParse(timeStr.AsSpan(0, 2), out var yearPart) ||
            !int.TryParse(timeStr.AsSpan(2, 2), out var month) || 
            !int.TryParse(timeStr.AsSpan(4, 2), out var day) ||
            !int.TryParse(timeStr.AsSpan(6, 2), out var hour) || 
            !int.TryParse(timeStr.AsSpan(8, 2), out var minute)) {
            throw new ArgumentException("Invalid time format", nameof(timeStr));
        }
        
        return new DateTime(2000 + yearPart, month, day, hour, minute, 0);
    }

    /// <summary>
    /// Parses path string (pipe-separated stations) to list.
    /// </summary>
    /// <param name="pathStr">The pipe-separated path string.</param>
    /// <returns>A list of station names.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<string> ParsePath(string? pathStr) => string.IsNullOrEmpty(pathStr) ? [] :
        pathStr.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
}
