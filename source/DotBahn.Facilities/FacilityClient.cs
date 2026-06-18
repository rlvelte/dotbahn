using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using DotBahn.Facilities.Internal.Contracts;
using DotBahn.Facilities.Internal.Transformers;
using DotBahn.Facilities.Models;
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
    [ActivatorUtilitiesConstructor]
    internal FacilityClient(HttpClient http, IAuthorization authorization, IParser<IEnumerable<FacilityContract>> parser, ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>> transformer)
        : base(http, authorization) {
        _parser = parser;
        _transformer = transformer;
    }

    /// <summary>
    /// Client for accessing 'Deutsche Bahn FaSta'-API.
    /// </summary>
    /// <remarks>
    /// Use only when instantiating manually without a DI container.
    /// </remarks>
    /// <param name="http">The HTTP client used for requests. The caller owns its lifecycle; it is not disposed by this instance.</param>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    public FacilityClient(HttpClient http, ClientOptions options, AuthorizationOptions auth) : base(http, options, auth) {
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
