using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for a transport-company multi-language
/// </summary>
[ExcludeFromCodeCoverage]
internal record TransportCompanyMultiLanguageContract {
    /// <summary>
    /// Gets the email
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the language
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the phone number
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Gets the short name
    /// </summary>
    [JsonPropertyName("shortName")]
    public string ShortName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the website
    /// </summary>
    [JsonPropertyName("website")]
    public string Website { get; init; } = string.Empty;

    /// <summary>
    /// Gets the website digital assistant
    /// </summary>
    [JsonPropertyName("websiteDigitalAssistant")]
    public string? WebsiteDigitalAssistant { get; init; }

    /// <summary>
    /// Gets the website ticketing
    /// </summary>
    [JsonPropertyName("websiteTicketing")]
    public string? WebsiteTicketing { get; init; }
}
