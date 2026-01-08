using System.Net;
using DotBahn.Clients.Facilities.Client;
using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Modules.Shared.Parsing;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Clients.Facilities;

/// <summary>
/// Extension methods for setting up FaSta (Station Facilities Status) services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Adds the FaSta client using HttpClientFactory, with options configured via callback.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddDotBahnFacilities(this IServiceCollection services, Action<IServiceProvider, ClientOptions> configuration) {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IConfigureOptions<ClientOptions>>(sp => new ConfigureOptions<ClientOptions>(opt => configuration(sp, opt)));
        services.AddOptions<ClientOptions>()
                .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseUri must be an absolute URI.")
                .ValidateOnStart();

        services.AddHttpClient<Client.FacilitiesClient>((sp, http) => {
            var options = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
            http.BaseAddress = options.BaseEndpoint;
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        services.AddSingleton<IParser<List<FacilityContract>>, JsonParser<List<FacilityContract>>>();
        services.AddSingleton<IParser<FacilityContract>, JsonParser<FacilityContract>>();
        services.AddSingleton<IParser<StationContract>, JsonParser<StationContract>>();

        return services;
    }
}
