using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for an error response.
/// </summary>
[ExcludeFromCodeCoverage]
internal record ErrorResponseContract {
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
    /// Gets the errors.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<ErrorDetailContract>? Errors { get; init; }

    /// <summary>
    /// Gets the instance identifier.
    /// </summary>
    [JsonPropertyName("instanceId")]
    public string? InstanceId { get; init; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;
}
