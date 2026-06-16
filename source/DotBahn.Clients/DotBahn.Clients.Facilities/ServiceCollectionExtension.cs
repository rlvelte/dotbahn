using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Facilities.Interfaces;
using DotBahn.Clients.Facilities.Transformer;
using DotBahn.Clients.Shared;
using DotBahn.Clients.Shared.Parsing;
using DotBahn.Clients.Shared.Parsing.Base;
using DotBahn.Data.Facilities.Models;
using DotBahn.Data.Shared.Transformer;

using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Clients.Facilities;

/// <summary>
/// Extension methods for setting up FaSta in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtension {
    private const string OptionsName = "DotBahn.Facilities";

    /// <summary>
    /// Adds the FaSta client using HttpClientFactory, with options configured via callback.
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnFacilities(this IServiceCollection services, Action<ClientOptions> configuration) {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDotBahnClient<IFacilitiesClient, FacilitiesClient>(OptionsName, configuration);

        services.AddSingleton<IParser<IEnumerable<FacilityContract>>, JsonParser<List<FacilityContract>>>();
        services.AddSingleton<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>, FacilityTransformer>();

        return services;
    }
}
