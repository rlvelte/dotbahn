namespace DotBahn.Boards;

/// <summary>
/// Query for an arrival or departure board.
/// </summary>
public sealed record BoardQuery {
    /// <summary>
    /// EVA numbers of stop places. Mutually exclusive with <see cref="StadaId"/> and <see cref="Dhids"/>.
    /// </summary>
    public IReadOnlyList<string>? EvaNumbers { get; init; }

    /// <summary>
    /// Stada ID of a station. Mutually exclusive with <see cref="EvaNumbers"/> and <see cref="Dhids"/>.
    /// </summary>
    public string? StadaId { get; init; }

    /// <summary>
    /// DHIDs of stop places. Mutually exclusive with <see cref="EvaNumbers"/> and <see cref="StadaId"/>.
    /// </summary>
    public IReadOnlyList<string>? Dhids { get; init; }

    /// <summary>
    /// Optional platform names to filter the board.
    /// </summary>
    public IReadOnlyList<string>? Platforms { get; init; }

    /// <summary>
    /// Board configuration modes to extend the board to nearby stop places.
    /// </summary>
    public IReadOnlyList<string>? BoardConfigurations { get; init; }

    /// <summary>
    /// Start of the time window. If omitted, the API uses the current time.
    /// </summary>
    public DateTime? Start { get; init; }

    /// <summary>
    /// End of the time window. Mutually exclusive with <see cref="EntryCount"/>.
    /// </summary>
    public DateTime? End { get; init; }

    /// <summary>
    /// Fixed number of entries to return from <see cref="Start"/>.
    /// <remarks>
    /// Mutually exclusive with <see cref="End"/>.
    /// </remarks>
    /// </summary>
    public int? EntryCount { get; init; }

    /// <summary>
    /// Transport types to include. If empty, the API defaults to rail transports.
    /// </summary>
    public IReadOnlyList<string> TransportTypes { get; init; } = [];

    /// <summary>
    /// Languages to request, as ISO-639-3 codes.
    /// </summary>
    public IReadOnlyList<string> Languages { get; init; } = ["deu", "eng"];

    /// <summary>
    /// Maximum number of entries returned per transport type.
    /// </summary>
    public int MaxEntriesPerTransportType { get; init; } = 25;

    /// <summary>
    /// Maximum number of via stops returned per journey.
    /// </summary>
    public int? MaxViaStops { get; init; }

    /// <summary>
    /// Whether to include line colors in the response.
    /// </summary>
    public bool IncludeLineColours { get; init; }

    /// <summary>
    /// Whether to include platform accessibility information in the response.
    /// </summary>
    public bool IncludePlatformAccessibility { get; init; }

    /// <summary>
    /// Whether to exclude canceled entries.
    /// </summary>
    public bool NoCancellations { get; init; }

    /// <summary>
    /// Whether to return only replacement transports.
    /// </summary>
    public bool OnlyReplacementTransports { get; init; }

    /// <summary>
    /// Sorting order, either <c>TIME</c> or <c>TIME_SCHEDULE</c>.
    /// </summary>
    public string SortBy { get; init; } = "TIME";

    /// <summary>
    /// Restricts the board to one or more EVA numbers.
    /// </summary>
    /// <param name="evaNumbers">EVA numbers of stop places.</param>
    /// <returns>A new query with the selector applied.</returns>
    public BoardQuery ForEvaNumbers(params string[] evaNumbers) => this with { EvaNumbers = evaNumbers };

    /// <summary>
    /// Restricts the board to a Stada station id.
    /// </summary>
    /// <param name="stadaId">Stada identifier.</param>
    /// <returns>A new query with the selector applied.</returns>
    public BoardQuery ForStada(string stadaId) => this with { StadaId = stadaId };

    /// <summary>
    /// Restricts the board to one or more DHIDs.
    /// </summary>
    /// <param name="dhids">DHIDs of stop places.</param>
    /// <returns>A new query with the selector applied.</returns>
    public BoardQuery ForDhids(params string[] dhids) => this with { Dhids = dhids };

    /// <summary>
    /// Filters the board to one or more platforms.
    /// </summary>
    /// <param name="platforms">Platform names.</param>
    /// <returns>A new query with the selector applied.</returns>
    public BoardQuery OnPlatforms(params string[] platforms) => this with { Platforms = platforms };

    /// <summary>
    /// Sets the board configuration modes for extending the board to nearby stop places.
    /// </summary>
    /// <param name="configurations">Configuration modes.</param>
    /// <returns>A new query with the selector applied.</returns>
    public BoardQuery WithBoardConfigurations(params string[] configurations) => this with { BoardConfigurations = configurations };

    /// <summary>
    /// Sets the start of the time window.
    /// </summary>
    /// <param name="start">Start date and time.</param>
    /// <returns>A new query with the time window applied.</returns>
    public BoardQuery StartingAt(DateTime start) => this with { Start = start };

    /// <summary>
    /// Sets the end of the time window.
    /// </summary>
    /// <param name="end">End date and time.</param>
    /// <returns>A new query with the time window applied.</returns>
    public BoardQuery EndingAt(DateTime end) => this with { End = end };

    /// <summary>
    /// Requests a fixed number of entries starting at <see cref="Start"/>.
    /// </summary>
    /// <param name="entryCount">Number of entries.</param>
    /// <returns>A new query with the time window applied.</returns>
    public BoardQuery ForNext(int entryCount) => this with { EntryCount = entryCount };

    /// <summary>
    /// Sets the transport types to include.
    /// </summary>
    /// <param name="transportTypes">Transport types.</param>
    /// <returns>A new query with the transport types applied.</returns>
    public BoardQuery WithTransportTypes(params string[] transportTypes) => this with { TransportTypes = transportTypes };

    /// <summary>
    /// Sets the requested languages.
    /// </summary>
    /// <param name="languages">ISO-639-3 language codes.</param>
    /// <returns>A new query with the languages applied.</returns>
    public BoardQuery InLanguages(params string[] languages) => this with { Languages = languages };

    /// <summary>
    /// Sets the maximum number of entries per transport type.
    /// </summary>
    /// <param name="max">Maximum entries.</param>
    /// <returns>A new query with the limit applied.</returns>
    public BoardQuery WithMaxEntriesPerTransportType(int max) => this with { MaxEntriesPerTransportType = max };

    /// <summary>
    /// Sets the maximum number of via stops per journey.
    /// </summary>
    /// <param name="max">Maximum via stops.</param>
    /// <returns>A new query with the limit applied.</returns>
    public BoardQuery WithMaxViaStops(int max) => this with { MaxViaStops = max };

    /// <summary>
    /// Requests line colors in the response.
    /// </summary>
    /// <returns>A new query with the flag applied.</returns>
    public BoardQuery WithLineColours() => this with { IncludeLineColours = true };

    /// <summary>
    /// Requests platform accessibility information in the response.
    /// </summary>
    /// <returns>A new query with the flag applied.</returns>
    public BoardQuery WithPlatformAccessibility() => this with { IncludePlatformAccessibility = true };

    /// <summary>
    /// Excludes canceled entries from the response.
    /// </summary>
    /// <returns>A new query with the flag applied.</returns>
    public BoardQuery WithoutCancellations() => this with { NoCancellations = true };

    /// <summary>
    /// Requests only replacement transports.
    /// </summary>
    /// <returns>A new query with the flag applied.</returns>
    public BoardQuery WithOnlyReplacementTransports() => this with { OnlyReplacementTransports = true };

    /// <summary>
    /// Sorts results by best-known time.
    /// </summary>
    /// <returns>A new query with the sort order applied.</returns>
    public BoardQuery SortByTime() => this with { SortBy = "TIME" };

    /// <summary>
    /// Sorts results by scheduled time.
    /// </summary>
    /// <returns>A new query with the sort order applied.</returns>
    public BoardQuery SortByTimeSchedule() => this with { SortBy = "TIME_SCHEDULE" };
}
