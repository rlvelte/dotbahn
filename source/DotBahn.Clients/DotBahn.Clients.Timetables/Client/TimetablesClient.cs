using DotBahn.Clients.Shared.Base;
using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Service.Base;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Parsing.Base;

namespace DotBahn.Clients.Timetables.Client;

/// <summary>
/// Client for accessing 'Deutsche Bahn Timetables'-API.
/// </summary>
public class TimetablesClient : ClientBase {
    private readonly IParser<TimetableResponseContract> _parser;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn Timetables'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    public TimetablesClient(HttpClient http, IAuthorization authorization, IParser<TimetableResponseContract> parser, ICache? cache = null) 
        : base(http, authorization, cache) {
        _parser = parser;
    }
    
    /// <summary>
    /// Client for accessing 'Deutsche Bahn Timetables'-API.
    /// </summary>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <param name="cache">The cache options for the client.</param>
    public TimetablesClient(ClientOptions options, AuthorizationOptions auth, CacheOptions? cache = null)
        : base(options, auth, cache) {
        _parser = new XmlParser<TimetableResponseContract>();
    }

    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="TimetableResponseContract"/> with current information.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<TimetableResponseContract> GetFullChangesAsync(int eva) =>
        await GetAsync($"/fchg/{eva}", _parser, "application/xml");

    /// <summary>
    /// Gets recent changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="TimetableResponseContract"/> with recent changes.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<TimetableResponseContract> GetRecentChangesAsync(int eva) => 
        await GetAsync($"/rchg/{eva}", _parser, "application/xml");

    /// <summary>
    /// Gets the planned timetable for a specific station and time.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="dateTime">The date and hour (only YYMMDD/HH are used).</param>
    /// <returns>A <see cref="TimetableResponseContract"/> for the specified hour.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<TimetableResponseContract> GetPlannedTimetableAsync(int eva, DateTime dateTime) {
        var dateStr = dateTime.ToString("yyMMdd");
        var hourStr = dateTime.ToString("HH");
        return await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", _parser, "application/xml");
    }
}
