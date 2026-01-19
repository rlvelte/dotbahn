using System.Net;
using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Timetables.Client;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Transformer;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Timetables.Models;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Parsing.Base;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Clients.Timetables;

/// <summary>
/// Extension methods for setting up Timetables services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Timetables";

    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the Timetables client using HttpClientFactory, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddDotBahnTimetables(Action<ClientOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.Configure(OptionsName, configuration);
            services.AddOptions<ClientOptions>(OptionsName)
                    .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseUri must be an absolute URI.")
                    .ValidateOnStart();

            services.AddHttpClient<TimetablesClient>((sp, http) => {
                var options = sp.GetRequiredService<IOptionsSnapshot<ClientOptions>>().Get(OptionsName);
                http.BaseAddress = options.BaseEndpoint;
                http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            
            services.AddSingleton<IParser<TimetableResponseContract>, XmlParser<TimetableResponseContract>>();
            
            services.AddSingleton<ITransformer<Timetable, TimetableResponseContract>, TimetableTransformer>();
            services.AddSingleton<IMerger<Timetable>, TimetableTransformer>();
        
            return services;
        }
    }
}
