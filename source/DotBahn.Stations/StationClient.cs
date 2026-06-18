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
/// Client for accessing 'Deutsche Bahn StaDa'-API.
/// </summary>
public class StationClient : ClientBase, IStationClient {
    private readonly IParser<StationsResponseContract> _parser;
    private readonly ITransformer<IEnumerable<Station>, StationsResponseContract> _transformer;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn StaDa'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="transformer">The transformer for this model and contract types.</param>
    [ActivatorUtilitiesConstructor]
    internal StationClient(HttpClient http, IAuthorization authorization, IParser<StationsResponseContract> parser, ITransformer<IEnumerable<Station>, StationsResponseContract> transformer)
        : base(http, authorization) {
        _parser = parser;
        _transformer = transformer;
    }

    /// <summary>
    /// Client for accessing 'Deutsche Bahn StaDa'-API.
    /// </summary>
    /// <remarks>
    /// Use only when instantiating manually without a DI container.
    /// </remarks>
    /// <param name="http">The HTTP client used for requests. The caller owns its lifecycle; it is not disposed by this instance.</param>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    public StationClient(HttpClient http, ClientOptions options, AuthorizationOptions auth)
        : base(http, options, auth) {
        _parser = new JsonParser<StationsResponseContract>();
        _transformer = new StationTransformer();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Station>> GetStationsAsync(StationQuery query, CancellationToken cancellation = default) {
        ArgumentNullException.ThrowIfNull(query);
        var response = await GetAsync("/stations", _parser, "application/json", query.ToQueryParameters(), cancellation).ConfigureAwait(false);
        response.Stations.Sort((first, second) => first.Category.CompareTo(second.Category));
        return [.. _transformer.Transform(response)];
    }
}
