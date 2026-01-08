using System.Net;
using DotBahn.Clients.Timetables.Client;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Options;
using DotBahn.Modules.Shared.Parsing;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

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
        /// <param name="configuration">Delegate to configure <see cref="TimetableOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddDotBahnTimetables(Action<IServiceProvider, TimetableOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
        
            services.AddSingleton<IConfigureOptions<TimetableOptions>>(sp => new ConfigureOptions<TimetableOptions>(opt => configuration(sp, opt)));
            services.AddOptions<TimetableOptions>()
                    .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseUri must be an absolute URI.")
                    .ValidateOnStart();

            services.AddHttpClient<TimetablesClient>((sp, http) => {
                var options = sp.GetRequiredService<IOptions<TimetableOptions>>().Value;
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
