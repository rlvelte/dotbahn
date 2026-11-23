using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a replacement transport
/// </summary>
[ExcludeFromCodeCoverage]
internal record ReplacementTransportContract {
    /// <summary>
    /// Gets the real type
    /// </summary>
    [JsonPropertyName("realType")]
    public string RealType { get; init; } = string.Empty;
}
