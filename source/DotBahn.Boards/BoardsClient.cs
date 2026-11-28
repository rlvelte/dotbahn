using DotBahn.Boards.Internal.Contracts;
using DotBahn.Boards.Internal.Json;
using DotBahn.Boards.Internal.Transformers;
using DotBahn.Boards.Internal.Utilities;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Boards;

/// <summary>
/// Client for accessing the Deutsche Bahn OpenBoards API.
/// </summary>
public class BoardsClient : ClientBase, IBoardsClient {
    private readonly IParser<ArrivalBoardResponseContract> _arrivalParser;
    private readonly IParser<DepartureBoardResponseContract> _departureParser;
    private readonly ITransformer<Board<ArrivalEntry>, ArrivalBoardResponseContract> _arrivalTransformer;
    private readonly ITransformer<Board<DepartureEntry>, DepartureBoardResponseContract> _departureTransformer;

    /// <inheritdoc />
    protected override string ApiName => "boards";

    /// <summary>
    /// Client for accessing the Deutsche Bahn OpenBoards API.
    /// </summary>
    /// <param name="http">The HTTP client used for requests.</param>
    /// <param name="authorization">The provider used for retrieving access tokens.</param>
    /// <param name="arrivalParser">The parser for arrival-board responses.</param>
    /// <param name="departureParser">The parser for departure-board responses.</param>
    /// <param name="arrivalTransformer">The transformer for arrival-board responses.</param>
    /// <param name="departureTransformer">The transformer for departure-board responses.</param>
    [ActivatorUtilitiesConstructor]
    internal BoardsClient(HttpClient http, IAuthorization authorization, IParser<ArrivalBoardResponseContract> arrivalParser, IParser<DepartureBoardResponseContract> departureParser, ITransformer<Board<ArrivalEntry>, ArrivalBoardResponseContract> arrivalTransformer, ITransformer<Board<DepartureEntry>, DepartureBoardResponseContract> departureTransformer) : base(http, authorization) {
        _arrivalParser = arrivalParser;
        _departureParser = departureParser;
        _arrivalTransformer = arrivalTransformer;
        _departureTransformer = departureTransformer;
    }

    /// <summary>
    /// Client for accessing the Deutsche Bahn OpenBoards API.
    /// </summary>
    /// <remarks>
    /// Creates and owns its own <see cref="HttpClient"/>. Dispose this instance to release it.
    /// Use only when instantiating manually without a DI container.
    /// </remarks>
    /// <param name="options">The options for this instance.</param>
    /// <param name="auth">The auth credentials for the client.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> or <paramref name="auth"/> is <c>null</c>.</exception>
    public BoardsClient(ClientOptions options, AuthorizationOptions auth) : base(options, auth) {
        _arrivalParser = new JsonParser<ArrivalBoardResponseContract>(BoardsJsonContext.Default.ArrivalBoardResponseContract);
        _departureParser = new JsonParser<DepartureBoardResponseContract>(BoardsJsonContext.Default.DepartureBoardResponseContract);
        _arrivalTransformer = new ArrivalBoardTransformer();
        _departureTransformer = new DepartureBoardTransformer();
    }

    /// <inheritdoc />
    public async Task<Board<ArrivalEntry>> GetArrivalsAsync(BoardQuery query, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(query);

        var request = QueryUtil.ToArrivalContract(query);
        var response = await PostAsync("/arrivals", request, BoardsJsonContext.Default.OpenArrivalBoardRequestContract, _arrivalParser, "application/json", ct).ConfigureAwait(false);

        return _arrivalTransformer.Transform(response);
    }

    /// <inheritdoc />
    public async Task<Board<DepartureEntry>> GetDeparturesAsync(BoardQuery query, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(query);

        var request = QueryUtil.ToDepartureContract(query);
        var response = await PostAsync("/departures", request, BoardsJsonContext.Default.OpenDepartureBoardRequestContract, _departureParser, "application/json", ct).ConfigureAwait(false);

        return _departureTransformer.Transform(response);
    }
}
