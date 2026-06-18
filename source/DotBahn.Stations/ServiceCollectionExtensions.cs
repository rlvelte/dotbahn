using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using DotBahn.Shared;
using DotBahn.Shared.Parsing;
using DotBahn.Shared.Transformer;
using DotBahn.Stations.Internal.Contracts;
using DotBahn.Stations.Internal.Transformers;
using DotBahn.Stations.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Stations;

/// <summary>
/// Extension methods for setting up StaDa in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Stations";

    private static readonly Uri DefaultStationsEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/");

    /// <summary>
    /// Adds the StaDa client using HttpClientFactory.
    /// <remarks>
    /// The <see cref="ClientOptions.BaseEndpoint"/> defaults to <c>https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/</c> and can be overridden in the optional delegate.
    /// </remarks>
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Optional delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnStations(this IServiceCollection services, Action<ClientOptions>? configuration = null) {
        services.AddOptions<ClientOptions>(OptionsName)
                .Configure(opts => {
                    opts.BaseEndpoint = DefaultStationsEndpoint;
                    configuration?.Invoke(opts);
                })
                .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseEndpoint must be an absolute URI.")
                .ValidateOnStart();

        services.AddHttpClient<IStationClient, StationClient>(OptionsName, (http, sp) => {
            var options = sp.GetRequiredService<IOptionsMonitor<ClientOptions>>().Get(OptionsName);
            http.BaseAddress = options.BaseEndpoint;
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");

            var auth = sp.GetRequiredService<IAuthorization>();
            var cache = sp.GetService<ICache>();
            var parser = sp.GetRequiredService<IParser<StationsResponseContract>>();
            var transformer = sp.GetRequiredService<ITransformer<IEnumerable<Station>, StationsResponseContract>>();
            return new StationClient(http, auth, parser, transformer, cache);
        });

        services.AddSingleton<IParser<StationsResponseContract>, JsonParser<StationsResponseContract>>();
        services.AddSingleton<IParser<StationContract>, JsonParser<StationContract>>();
        services.AddSingleton<ITransformer<IEnumerable<Station>, StationsResponseContract>, StationTransformer>();

        return services;
    }
}
