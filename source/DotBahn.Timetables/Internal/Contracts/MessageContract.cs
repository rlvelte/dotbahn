using System.Diagnostics.CodeAnalysis;

namespace DotBahn.Timetables.Internal.Contracts;

/// <summary>
/// Raw message/disruption information
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessageContract {
    /// <summary>
    /// Message ID
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Valid from timestamp (YYMMDDhhmm)
    /// </summary>
    public string? ValidFrom { get; init; }

    /// <summary>
    /// Valid to timestamp (YYMMDDhhmm)
    /// </summary>
    public string? ValidTo { get; init; }

    /// <summary>
    /// Message code
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Internal message flag
    /// </summary>
    public string? IsInternal { get; init; }

    /// <summary>
    /// Deleted flag
    /// </summary>
    public string? IsDeleted { get; init; }

    /// <summary>
    /// External category
    /// </summary>
    public string? ExternalCategory { get; init; }

    /// <summary>
    /// Timestamp (YYMMDDhhmm)
    /// </summary>
    public string? Timestamp { get; init; }

    /// <summary>
    /// Priority
    /// </summary>
    public string? Priority { get; init; }

    /// <summary>
    /// Message owner
    /// </summary>
    public string? Owner { get; init; }

    /// <summary>
    /// Category
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Message text content
    /// </summary>
    public string? Text { get; init; }
}
