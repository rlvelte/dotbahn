using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using DotBahn.Stations.Internal.Contracts;
using DotBahn.Stations.Internal.Transformers;
using DotBahn.Stations.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Stations;

/// <summary>
/// Client for accessing 'Deutsche Bahn StaDa'-API
/// </summary>
public class StationClient : ClientBase, IStationClient {
    private readonly IParser<StationsResponseContract> _parser;
    private readonly ITransformer<IEnumerable<Station>, StationsResponseContract> _transformer;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn StaDa'-API
    /// </summary>
    /// <param name="http">The HTTP client used for requests</param>
    /// <param name="authorization">The provider used for retrieving access tokens</param>
    /// <param name="parser">The parser for this contract type</param>
    /// <param name="transformer">The transformer for this model and contract types</param>
    [ActivatorUtilitiesConstructor]
    internal StationClient(HttpClient http, IAuthorization authorization, IParser<StationsResponseContract> parser, ITransformer<IEnumerable<Station>, StationsResponseContract> transformer)
        : base(http, authorization) {
        _parser = parser;
        _transformer = transformer;
    }

    /// <summary>
    /// Client for accessing 'Deutsche Bahn StaDa'-API
    /// </summary>
    /// <remarks>
    /// Creates and owns its own <see cref="HttpClient"/>. Dispose this instance to release it.
    /// Use only when instantiating manually without a DI container
    /// </remarks>
    /// <param name="options">The options for this instance</param>
    /// <param name="auth">The auth credentials for the client</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c></exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="auth"/> is <c>null</c></exception>
    public StationClient(ClientOptions options, AuthorizationOptions auth)
        : base(options, auth) {
        _parser = new JsonParser<StationsResponseContract>();
        _transformer = new StationTransformer();
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c></exception>
    public async Task<IReadOnlyList<Station>> GetStationsAsync(StationQuery query, CancellationToken cancellation = default) {
        ArgumentNullException.ThrowIfNull(query);
        var response = await GetAsync("/stations", _parser, "application/json", query.ToQueryParameters(), cancellation).ConfigureAwait(false);
        response.Stations.Sort((first, second) => first.Category.CompareTo(second.Category));
        return [.. _transformer.Transform(response)];
    }
}
