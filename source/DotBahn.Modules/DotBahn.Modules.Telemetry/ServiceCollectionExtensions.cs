using DotBahn.Modules.Telemetry.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace DotBahn.Modules.Telemetry;

/// <summary>
/// Extension methods for setting up DotBahn telemetry services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds DotBahn telemetry services with custom options.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="TelemetryOptions"/>.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddDotBahnTelemetry(Action<TelemetryOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.Configure(configuration);
            services.AddOptions<TelemetryOptions>();
            
            services.AddTransient<TelemetryHttpMessageHandler>();
            services.ConfigureAll<HttpClientFactoryOptions>(httpOptions => {
                httpOptions.HttpMessageHandlerBuilderActions.Add(builder => {
                    builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<TelemetryHttpMessageHandler>());
                });
            });
            
            return services;
        }
    }
}
