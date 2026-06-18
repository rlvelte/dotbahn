using DotBahn.Facilities.Contracts;
using DotBahn.Facilities.Models;
using DotBahn.Shared;
using DotBahn.Shared.Parsing;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using DotBahn.Shared.Transformer;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Facilities;

/// <summary>
/// Client for accessing 'Deutsche Bahn FaSta'-API.
/// </summary>
public class FacilityClient : ClientBase, IFacilityClient {
    private readonly IParser<IEnumerable<FacilityContract>> _parser;
    private readonly ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>> _transformer;

    /// <summary>
    /// Client for accessing 'Deutsche Bahn FaSta'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="parser">The parser for this contract type.</param>
    /// <param name="transformer">The transformer for this model and contract types.</param>
    /// <param name="cache">The cache provider for storing requests.</param>
    [ActivatorUtilitiesConstructor]
    public FacilityClient(HttpClient http, IAuthorization authorization, IParser<IEnumerable<FacilityContract>> parser, ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>> transformer, ICache? cache = null)
        : base(http, authorization, cache) {
        _parser = parser;
        _transformer = transformer;
    }

    /// <summary>
    /// Client for accessing 'Deutsche Bahn FaSta'-API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests. The caller owns its lifecycle; it is not disposed by this instance.</param>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <param name="cache">The cache options for the client.</param>
    public FacilityClient(HttpClient http, ClientOptions options, AuthorizationOptions auth, CacheOptions? cache = null)
        : base(http, options, auth, cache) {
        _parser = new JsonParser<List<FacilityContract>>();
        _transformer = new FacilityTransformer();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Facility>> GetFacilitiesAsync(FacilityQuery query, CancellationToken cancellation = default) {
        ArgumentNullException.ThrowIfNull(query);
        var result = (await GetAsync("/facilities", _parser, "application/json", query.ToQueryParameters(), cancellation).ConfigureAwait(false)).ToList();
        return [.. _transformer.Transform(result)];
    }
}
