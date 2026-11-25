using DotBahn.Boards.Internal.Contracts;
using DotBahn.Boards.Internal.Json;
using DotBahn.Boards.Internal.Transformers;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Boards;

/// <summary>
/// Extension methods for registering the DotBahn Boards client.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Boards";
    private static readonly Uri DefaultBoardsEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/open-boards/v1/");

    /// <summary>
    /// Registers the <see cref="IBoardsClient"/> with its parser and transformer services.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="configuration">Optional action to override default client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDotBahnBoards(this IServiceCollection services, Action<ClientOptions>? configuration = null) {
        services.AddOptions<ClientOptions>(OptionsName)
                .Configure(opts => {
                    opts.BaseEndpoint = DefaultBoardsEndpoint;
                    configuration?.Invoke(opts);
                })
                .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseEndpoint must be an absolute URI.")
                .ValidateOnStart();

        services.AddHttpClient<IBoardsClient, BoardsClient>(OptionsName, (http, sp) => {
            var options = sp.GetRequiredService<IOptionsMonitor<ClientOptions>>().Get(OptionsName);
            http.BaseAddress = options.BaseEndpoint;
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/2.X (+https://github.com/rlvelte/dotbahn)");

            var auth = sp.GetRequiredService<IAuthorization>();
            var arrivalParser = sp.GetRequiredService<IParser<ArrivalBoardResponseContract>>();
            var departureParser = sp.GetRequiredService<IParser<DepartureBoardResponseContract>>();
            var arrivalTransformer = sp.GetRequiredService<ITransformer<Board<ArrivalEntry>, ArrivalBoardResponseContract>>();
            var departureTransformer = sp.GetRequiredService<ITransformer<Board<DepartureEntry>, DepartureBoardResponseContract>>();

            return new BoardsClient(http, auth, arrivalParser, departureParser, arrivalTransformer, departureTransformer);
        })
        .AddStandardResilienceHandler();

        services.AddSingleton<IParser<ArrivalBoardResponseContract>>(_ => new JsonParser<ArrivalBoardResponseContract>(BoardsJsonContext.Default.ArrivalBoardResponseContract));
        services.AddSingleton<IParser<DepartureBoardResponseContract>>(_ => new JsonParser<DepartureBoardResponseContract>(BoardsJsonContext.Default.DepartureBoardResponseContract));
        services.AddSingleton<ITransformer<Board<ArrivalEntry>, ArrivalBoardResponseContract>, ArrivalBoardTransformer>();
        services.AddSingleton<ITransformer<Board<DepartureEntry>, DepartureBoardResponseContract>, DepartureBoardTransformer>();

        return services;
    }
}
