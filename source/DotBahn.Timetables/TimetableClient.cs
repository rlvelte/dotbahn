using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using DotBahn.Timetables.Internal.Contracts;
using DotBahn.Timetables.Internal.Parsing;
using DotBahn.Timetables.Internal.Transformers;
using DotBahn.Timetables.Models;

using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Timetables;

/// <summary>
/// Client for accessing 'Deutsche Bahn Timetables'-API.
/// </summary>
public class TimetableClient : ClientBase, ITimetableClient {
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
    [ActivatorUtilitiesConstructor]
    internal TimetableClient(HttpClient http, IAuthorization authorization, IParser<TimetableResponseContract> parser, ITransformer<Timetable, TimetableResponseContract> transformer, IMerger<Timetable> merger)
        : base(http, authorization) {
        _parser = parser;
        _transformer = transformer;
        _merger = merger;
    }

    /// <summary>
    /// Client for accessing 'Deutsche Bahn Timetables'-API.
    /// </summary>
    /// <remarks>
    /// Use only when instantiating manually without a DI container.
    /// </remarks>
    /// <param name="http">The HTTP client used for requests. The caller owns its lifecycle; it is not disposed by this instance.</param>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    public TimetableClient(HttpClient http, ClientOptions options, AuthorizationOptions auth) : base(http, options, auth) {
        _parser = new TimetableXmlParser();
        _transformer = new TimetableTransformer();
        _merger = new TimetableMerger();
    }

    /// <inheritdoc />
    public async Task<Timetable> GetTimetableAsync(int eva, DateTime dateTime, CancellationToken cancellation = default) {
        var dateStr = dateTime.ToString("yyMMdd");
        var hourStr = dateTime.ToString("HH");
        var response = await GetAsync($"/plan/{eva}/{dateStr}/{hourStr}", _parser, "application/xml", null, cancellation).ConfigureAwait(false);
        return _transformer.Transform(response);
    }

    /// <inheritdoc />
    public async Task<Timetable> GetFullChangesAsync(int eva, Timetable? mergeOn = null, CancellationToken cancellation = default) {
        var response = await GetAsync($"/fchg/{eva}", _parser, "application/xml", null, cancellation).ConfigureAwait(false);
        var changes = _transformer.Transform(response);
        return mergeOn != null ? _merger.Merge(mergeOn, changes) : changes;
    }

    /// <inheritdoc />
    public async Task<Timetable> GetRecentChangesAsync(int eva, Timetable? mergeOn = null, CancellationToken cancellation = default) {
        var response = await GetAsync($"/rchg/{eva}", _parser, "application/xml", null, cancellation).ConfigureAwait(false);
        var changes = _transformer.Transform(response);
        return mergeOn != null ? _merger.Merge(mergeOn, changes) : changes;
    }
}
