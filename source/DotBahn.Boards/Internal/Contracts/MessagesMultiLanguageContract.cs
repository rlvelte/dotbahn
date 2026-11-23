using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a messages multi-language
/// </summary>
[ExcludeFromCodeCoverage]
internal record MessagesMultiLanguageContract {
    /// <summary>
    /// Gets the attributes
    /// </summary>
    [JsonPropertyName("attributes")]
    public List<MessageAttributeMultiLanguageContract>? Attributes { get; init; }

    /// <summary>
    /// Gets the disruptions
    /// </summary>
    [JsonPropertyName("disruptions")]
    public List<MessageDisruptionCommunicationMultiLanguageContract>? Disruptions { get; init; }

    /// <summary>
    /// Gets the notes
    /// </summary>
    [JsonPropertyName("notes")]
    public List<MessageNoteMultiLanguageContract>? Notes { get; init; }

    /// <summary>
    /// Gets the RIS cause codes
    /// </summary>
    [JsonPropertyName("risCauseCodes")]
    public List<MessageRisCauseCodeMultiLanguageContract>? RisCauseCodes { get; init; }

    /// <summary>
    /// Gets the RIS quality deviations
    /// </summary>
    [JsonPropertyName("risQualityDeviations")]
    public List<MessageRisQualityDeviationMultiLanguageContract>? RisQualityDeviations { get; init; }
}
