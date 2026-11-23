using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Common.Parsing.Converters;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a platform accessibility
/// </summary>
[ExcludeFromCodeCoverage]
internal record PlatformAccessibilityContract {
    /// <summary>
    /// Gets a value indicating whether audible signals are available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("audibleSignalsAvailable")]
    public bool AudibleSignalsAvailable { get; init; }

    /// <summary>
    /// Gets a value indicating whether an automatic door is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("automaticDoor")]
    public bool AutomaticDoor { get; init; }

    /// <summary>
    /// Gets a value indicating whether boarding aid is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("boardingAid")]
    public bool BoardingAid { get; init; }

    /// <summary>
    /// Gets a value indicating whether a passenger information display is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("passengerInformationDisplay")]
    public bool PassengerInformationDisplay { get; init; }

    /// <summary>
    /// Gets a value indicating whether a platform sign is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("platformSign")]
    public bool PlatformSign { get; init; }

    /// <summary>
    /// Gets a value indicating whether stairs marking is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("stairsMarking")]
    public bool StairsMarking { get; init; }

    /// <summary>
    /// Gets a value indicating whether a standard platform height is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("standardPlatformHeight")]
    public bool StandardPlatformHeight { get; init; }

    /// <summary>
    /// Gets a value indicating whether step-free access is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("stepFreeAccess")]
    public bool StepFreeAccess { get; init; }

    /// <summary>
    /// Gets a value indicating whether tactile guiding strips are available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("tactileGuidingStrips")]
    public bool TactileGuidingStrips { get; init; }

    /// <summary>
    /// Gets a value indicating whether a tactile handrail label is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("tactileHandrailLabel")]
    public bool TactileHandrailLabel { get; init; }

    /// <summary>
    /// Gets a value indicating whether tactile platform access is available
    /// </summary>
    [JsonConverter(typeof(BahnDialectJsonConverter))]
    [JsonPropertyName("tactilePlatformAccess")]
    public bool TactilePlatformAccess { get; init; }
}
