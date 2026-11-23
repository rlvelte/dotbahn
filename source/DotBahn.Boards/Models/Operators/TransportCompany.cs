namespace DotBahn.Boards.Models.Operators;

/// <summary>
/// Transport company information for an administration.
/// </summary>
public class TransportCompany {
    /// <summary>
    /// Language of the entry, as ISO-639-3 code.
    /// </summary>
    public required string Language { get; init; }

    /// <summary>
    /// Name of the transport company.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Short name of the transport company.
    /// </summary>
    public required string ShortName { get; init; }

    /// <summary>
    /// Email address of the transport company.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Phone number of the transport company.
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Website of the transport company.
    /// </summary>
    public string? Website { get; init; }

    /// <summary>
    /// Ticketing website of the transport company.
    /// </summary>
    public string? WebsiteTicketing { get; init; }

    /// <summary>
    /// Website of the digital assistant of the transport company.
    /// </summary>
    public string? WebsiteDigitalAssistant { get; init; }
}
