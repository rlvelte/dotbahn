using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a time window.
/// </summary>
[ExcludeFromCodeCoverage]
internal record TimeWindowContract {
    /// <summary>
    /// Gets the entry count.
    /// </summary>
    [JsonPropertyName("entryCount")]
    public int? EntryCount { get; init; }

    /// <summary>
    /// Gets the time end.
    /// </summary>
    [JsonPropertyName("timeEnd")]
    public string? TimeEnd { get; init; }

    /// <summary>
    /// Gets the time start.
    /// </summary>
    [JsonPropertyName("timeStart")]
    public string? TimeStart { get; init; }
}
