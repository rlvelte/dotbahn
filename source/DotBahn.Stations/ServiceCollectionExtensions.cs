using DotBahn.Shared;
using DotBahn.Shared.Parsing;
using DotBahn.Shared.Transformer;
using DotBahn.Stations.Contracts;
using DotBahn.Stations.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Stations;

/// <summary>
/// Extension methods for setting up StaDa in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Stations";

    private static readonly Uri DefaultStationsEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/");

    /// <summary>
    /// Adds the StaDa client using HttpClientFactory.
    /// The <see cref="ClientOptions.BaseEndpoint"/> defaults to <c>https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/</c>
    /// and can be overridden in the optional delegate.
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Optional delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnStations(this IServiceCollection services, Action<ClientOptions>? configuration = null) {
        services.AddDotBahnClient<IStationClient, StationClient>(OptionsName, opts => {
            opts.BaseEndpoint = DefaultStationsEndpoint;
            configuration?.Invoke(opts);
        });

        services.AddSingleton<IParser<StationsResponseContract>, JsonParser<StationsResponseContract>>();
        services.AddSingleton<IParser<StationContract>, JsonParser<StationContract>>();
        services.AddSingleton<ITransformer<IEnumerable<Station>, StationsResponseContract>, StationTransformer>();

        return services;
    }
}
