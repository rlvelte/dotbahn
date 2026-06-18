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
    /// Creates and owns its own <see cref="HttpClient"/>. Dispose this instance to release it.
    /// Use only when instantiating manually without a DI container.
    /// </remarks>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="auth"/> is <c>null</c>.</exception>
    public FacilityClient(ClientOptions options, AuthorizationOptions auth) : base(options, auth) {
        _parser = new JsonParser<List<FacilityContract>>();
        _transformer = new FacilityTransformer();
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c>.</exception>
    public async Task<IReadOnlyList<Facility>> GetFacilitiesAsync(FacilityQuery query, CancellationToken cancellation = default) {
        ArgumentNullException.ThrowIfNull(query);
        var result = (await GetAsync("/facilities", _parser, "application/json", query.ToQueryParameters(), cancellation).ConfigureAwait(false)).ToList();
        return [.. _transformer.Transform(result)];
    }
}
