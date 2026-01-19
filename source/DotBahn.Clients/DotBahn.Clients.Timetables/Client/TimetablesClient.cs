using DotBahn.Clients.Shared.Client;
using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Transformer;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Timetables.Models;
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
    private readonly ITransformer<Timetable, TimetableResponseContract> _transformer;
    private readonly IMerger<Timetable> _merger;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn Timetables'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="transformer">The transformer for this model and contract types.</param>
    /// <param name="merger">The merger for the target type.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    public TimetablesClient(HttpClient http, IAuthorization authorization, IParser<TimetableResponseContract> parser, ITransformer<Timetable, TimetableResponseContract> transformer, IMerger<Timetable> merger, ICache? cache = null) 
        : base(http, authorization, cache) {
        _parser = parser;
        _transformer = transformer;
        _merger = merger;
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
        _transformer = new TimetableTransformer();
        _merger = new TimetableTransformer();
    }

    /// <summary>
    /// Gets full changes for a specific station.
    /// If there is a <see cref="Timetable"/> suppied the changes get merged onto this instance.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="current">Current timetable on which these changes should apply on.</param>
    /// <returns>A <see cref="Timetable"/> with full change information.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<Timetable> GetFullChangesAsync(int eva, Timetable? current = null) {
        var response = await GetAsync($"/fchg/{eva}", _parser, "application/xml");
        var changes = _transformer.Transform(response);
        
        return current != null ? _merger.Merge(current, changes) : changes;
    }

    /// <summary>
    /// Gets recent changes for a specific station.
    /// If there is a <see cref="Timetable"/> suppied the changes get merged onto this instance.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="current">Current timetable on which these changes should apply on.</param>
    /// <returns>A <see cref="Timetable"/> with recent change information.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<Timetable> GetRecentChangesAsync(int eva, Timetable? current = null) {
        var response = await GetAsync($"/rchg/{eva}", _parser, "application/xml");
        var changes = _transformer.Transform(response);
        
        return current != null ? _merger.Merge(current, changes) : changes;
    }
    
    /// <summary>
    /// Gets the timetable for a specific station and time.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <param name="dateTime">The date and hour (only YYMMDD/HH are used).</param>
    /// <returns>A <see cref="Timetable"/> for the specified hour.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    public async Task<Timetable> GetTimetableAsync(int eva, DateTime dateTime) {
        var dateStr = dateTime.ToString("yyMMdd");
        var hourStr = dateTime.ToString("HH");
        
        var response = await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", _parser, "application/xml");
        return _transformer.Transform(response);
    }
}
