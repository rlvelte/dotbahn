using System.Net;
using DotBahn.Clients.Facilities.Client;
using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Facilities.Transformer;
using DotBahn.Clients.Shared.Extensions;
using DotBahn.Clients.Shared.Options;
using DotBahn.Data.Facilities.Models;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Parsing.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Clients.Facilities;

/// <summary>
/// Extension methods for setting up FaSta (Station Facilities Status) services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Facilities";

    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the FaSta client using HttpClientFactory, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddDotBahnFacilities(Action<ClientOptions> configuration) {
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

            services.AddHttpClient<FacilitiesClient>((sp, http) => {
                var opt = sp.GetRequiredService<IOptionsSnapshot<ClientOptions>>().Get(OptionsName);
                http.BaseAddress = opt.BaseEndpoint;
                http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            services.AddSingleton<IParser<IEnumerable<FacilityContract>>, JsonParser<List<FacilityContract>>>();
            services.AddSingleton<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>, FacilityTransformer>();
            
            return services;
        }
    }
}
