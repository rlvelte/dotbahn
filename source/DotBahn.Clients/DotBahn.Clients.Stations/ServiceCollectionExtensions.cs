using System.Net;
using DotBahn.Clients.Shared.Extensions;
using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Stations.Client;
using DotBahn.Clients.Stations.Contracts;
using DotBahn.Clients.Stations.Transformer;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Stations.Models;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Parsing.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Clients.Stations;

/// <summary>
/// Extension methods for setting up StaDa (StationData) services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Stations";

    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the StaDa client using HttpClientFactory, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddDotBahnStations(Action<ClientOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            
            var options = new ClientOptions {
                BaseEndpoint = null!
            };
            
            configuration(options);
            services.EnsureAuthorization(options.ClientId, options.ApiKey);

            services.Configure(OptionsName, configuration);
            services.AddOptions<ClientOptions>(OptionsName)
                    .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseEndpoint must be an absolute URI.")
                    .ValidateOnStart();

            services.AddHttpClient<StationsClient>((sp, http) => {
                var opt = sp.GetRequiredService<IOptionsSnapshot<ClientOptions>>().Get(OptionsName);
                http.BaseAddress = opt.BaseEndpoint;
                http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            services.AddSingleton<IParser<StationsResponseContract>, JsonParser<StationsResponseContract>>();
            services.AddSingleton<IParser<StationContract>, JsonParser<StationContract>>();
            services.AddSingleton<ITransformer<IEnumerable<Station>, StationsResponseContract>, StationTransformer>();
        
            return services;
        }
    }
}
