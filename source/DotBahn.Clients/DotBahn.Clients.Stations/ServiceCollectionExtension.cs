using DotBahn.Clients.Shared;
using DotBahn.Clients.Shared.Parsing;
using DotBahn.Clients.Shared.Parsing.Base;
using DotBahn.Clients.Stations.Contracts;
using DotBahn.Clients.Stations.Interfaces;
using DotBahn.Clients.Stations.Transformer;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Stations.Models;

using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Clients.Stations;

/// <summary>
/// Extension methods for setting up StaDa in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtension {
    private const string OptionsName = "DotBahn.Stations";

    /// <summary>
    /// Adds the StaDa client using HttpClientFactory, with options configured via callback.
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnStations(this IServiceCollection services, Action<ClientOptions> configuration) {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDotBahnClient<IStationsClient, StationsClient>(OptionsName, configuration);

        services.AddSingleton<IParser<StationsResponseContract>, JsonParser<StationsResponseContract>>();
        services.AddSingleton<IParser<StationContract>, JsonParser<StationContract>>();
        services.AddSingleton<ITransformer<IEnumerable<Station>, StationsResponseContract>, StationTransformer>();

        return services;
    }
}
