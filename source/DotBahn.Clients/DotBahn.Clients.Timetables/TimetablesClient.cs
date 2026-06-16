using DotBahn.Clients.Shared;
using DotBahn.Clients.Shared.Parsing;
using DotBahn.Clients.Shared.Parsing.Base;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Interfaces;
using DotBahn.Clients.Timetables.Transformer;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Timetables.Models;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service.Base;
using DotBahn.Modules.Cache;
using DotBahn.Modules.Cache.Service.Base;

namespace DotBahn.Clients.Timetables;

/// <summary>
/// Client for accessing 'Deutsche Bahn Timetables'-API.
/// </summary>
public class TimetablesClient : ClientBase, ITimetablesClient {
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
    /// <param name="http">The HTTP client used for requests. The caller owns its lifecycle; it is not disposed by this instance.</param>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <param name="cache">The cache options for the client.</param>
    public TimetablesClient(HttpClient http, ClientOptions options, AuthorizationOptions auth, CacheOptions? cache = null)
        : base(http, options, auth, cache) {
        _parser = new XmlParser<TimetableResponseContract>();
        _transformer = new TimetableTransformer();
        _merger = new TimetableMerger();
    }

    /// <inheritdoc />
    public async Task<Timetable> GetFullChangesAsync(int eva, Timetable? current = null, CancellationToken cancellation = default) {
        var response = await GetAsync($"/fchg/{eva}", _parser, "application/xml", null, cancellation).ConfigureAwait(false);
        var changes = _transformer.Transform(response);
        return current != null ? _merger.Merge(current, changes) : changes;
    }

    /// <inheritdoc />
    public async Task<Timetable> GetRecentChangesAsync(int eva, Timetable? current = null, CancellationToken cancellation = default) {
        var response = await GetAsync($"/rchg/{eva}", _parser, "application/xml", null, cancellation).ConfigureAwait(false);
        var changes = _transformer.Transform(response);
        return current != null ? _merger.Merge(current, changes) : changes;
    }

    /// <inheritdoc />
    public async Task<Timetable> GetTimetableAsync(int eva, DateTime dateTime, CancellationToken cancellation = default) {
        var dateStr = dateTime.ToString("yyMMdd");
        var hourStr = dateTime.ToString("HH");
        var response = await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", _parser, "application/xml", null, cancellation).ConfigureAwait(false);
        return _transformer.Transform(response);
    }
}
