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
/// Client for accessing 'Deutsche Bahn Timetables'-API
/// </summary>
public class TimetableClient : ClientBase, ITimetableClient {
    private readonly IParser<TimetableResponseContract> _parser;
    private readonly ITransformer<Timetable, TimetableResponseContract> _transformer;
    private readonly IMerger<Timetable> _merger;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn Timetables'-API
    /// </summary>
    /// <param name="http">The HTTP client used for requests</param>
    /// <param name="authorization">The provider used for retrieving access tokens</param>
    /// <param name="parser">The parser for this contract type</param>
    /// <param name="transformer">The transformer for this model and contract types</param>
    /// <param name="merger">The merger for the target type</param>
    [ActivatorUtilitiesConstructor]
    internal TimetableClient(HttpClient http, IAuthorization authorization, IParser<TimetableResponseContract> parser, ITransformer<Timetable, TimetableResponseContract> transformer, IMerger<Timetable> merger)
        : base(http, authorization) {
        _parser = parser;
        _transformer = transformer;
        _merger = merger;
    }

    /// <summary>
    /// Client for accessing 'Deutsche Bahn Timetables'-API
    /// </summary>
    /// <remarks>
    /// Creates and owns its own <see cref="HttpClient"/>. Dispose this instance to release it.
    /// Use only when instantiating manually without a DI container
    /// </remarks>
    /// <param name="options">The options for this instance</param>
    /// <param name="auth">The auth credentials for the client</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c></exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="auth"/> is <c>null</c></exception>
    public TimetableClient(ClientOptions options, AuthorizationOptions auth) : base(options, auth) {
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
