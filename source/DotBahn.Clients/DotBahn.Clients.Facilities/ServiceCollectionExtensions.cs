using System.Net;
using DotBahn.Clients.Facilities.Client;
using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Facilities.Options;
using DotBahn.Modules.Shared.Parsing;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace DotBahn.Clients.Facilities;

/// <summary>
/// Extension methods for setting up FaSta (Station Facilities Status) services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Adds the FaSta client using HttpClientFactory, with options configured via callback.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">Delegate to configure <see cref="FaStaOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddDotBahnFacilities(this IServiceCollection services, Action<IServiceProvider, FaStaOptions> configuration) {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IConfigureOptions<FaStaOptions>>(sp => new ConfigureOptions<FaStaOptions>(opt => configuration(sp, opt)));
        services.AddOptions<FaStaOptions>()
                .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseUri must be an absolute URI.")
                .ValidateOnStart();

        services.AddHttpClient<FacilityClient>((sp, http) => {
            var options = sp.GetRequiredService<IOptions<FaStaOptions>>().Value;
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
