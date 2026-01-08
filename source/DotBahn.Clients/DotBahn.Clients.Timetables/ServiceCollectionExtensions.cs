using System.Net;
using DotBahn.Clients.Timetables.Client;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Modules.Shared.Parsing;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Clients.Timetables;

/// <summary>
/// Extension methods for setting up Timetables services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the Timetables client using HttpClientFactory, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddDotBahnTimetables(Action<IServiceProvider, ClientOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
        
            services.AddSingleton<IConfigureOptions<ClientOptions>>(sp => new ConfigureOptions<ClientOptions>(opt => configuration(sp, opt)));
            services.AddOptions<ClientOptions>()
                    .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseUri must be an absolute URI.")
                    .ValidateOnStart();

            services.AddHttpClient<Client.TimetablesClient>((sp, http) => {
                var options = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
                http.BaseAddress = options.BaseEndpoint;
                http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            
            services.AddSingleton<IParser<TimetableResponseContract>, XmlParser<TimetableResponseContract>>();
        
            return services;
        }
    }
}
