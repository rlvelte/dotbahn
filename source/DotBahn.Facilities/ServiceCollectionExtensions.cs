using DotBahn.Facilities.Contracts;
using DotBahn.Facilities.Models;
using DotBahn.Shared;
using DotBahn.Shared.Parsing;
using DotBahn.Shared.Transformer;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Facilities;

/// <summary>
/// Extension methods for setting up FaSta in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Facilities";

    private static readonly Uri DefaultFacilitiesEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/");

    /// <summary>
    /// Adds the FaSta client using HttpClientFactory.
    /// The <see cref="ClientOptions.BaseEndpoint"/> defaults to <c>https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/</c>
    /// and can be overridden in the optional delegate.
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Optional delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnFacilities(this IServiceCollection services, Action<ClientOptions>? configuration = null) {
        services.AddDotBahnClient<IFacilityClient, FacilityClient>(OptionsName, opts => {
            opts.BaseEndpoint = DefaultFacilitiesEndpoint;
            configuration?.Invoke(opts);
        });

        services.AddSingleton<IParser<IEnumerable<FacilityContract>>, JsonParser<List<FacilityContract>>>();
        services.AddSingleton<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>, FacilityTransformer>();

        return services;
    }
}
