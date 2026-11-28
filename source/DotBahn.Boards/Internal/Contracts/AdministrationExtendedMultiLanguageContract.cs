using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DotBahn.Boards.Internal.Contracts;

/// <summary>
/// Raw structure for an extended administration multi-language.
/// </summary>
[ExcludeFromCodeCoverage]
internal record AdministrationExtendedMultiLanguageContract {
    /// <summary>
    /// Gets the administration identifier.
    /// </summary>
    [JsonPropertyName("administrationId")]
    public string AdministrationId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the operator code.
    /// </summary>
    [JsonPropertyName("operatorCode")]
    public string OperatorCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the operator name.
    /// </summary>
    [JsonPropertyName("operatorName")]
    public string OperatorName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the transport companies.
    /// </summary>
    [JsonPropertyName("transportCompanies")]
    public List<TransportCompanyMultiLanguageContract>? TransportCompanies { get; init; }
}
