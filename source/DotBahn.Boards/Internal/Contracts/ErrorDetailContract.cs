using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for an error detail.
/// </summary>
[ExcludeFromCodeCoverage]
internal record ErrorDetailContract {
    /// <summary>
    /// Gets the detail.
    /// </summary>
    [JsonPropertyName("detail")]
    public string Detail { get; init; } = string.Empty;

    /// <summary>
    /// Gets the error code.
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the field.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; init; } = string.Empty;

    /// <summary>
    /// Gets the title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;
}
