using DotBahn.Modules.Authorization.Enumerations;
using DotBahn.Modules.Authorization.Service;
using DotBahn.Modules.Authorization.Service.Base;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Modules.Authorization;

/// <summary>
/// Extension methods for setting up authorization services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the authorization system, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="ModuleOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddAuthorizationProvider(Action<IServiceProvider, ModuleOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddSingleton<IConfigureOptions<ModuleOptions>>(sp => new ConfigureOptions<ModuleOptions>(opt => configuration(sp, opt)));
            services.AddOptions<ModuleOptions>()
                    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "DotBahn: 'ClientId' can't be null or empty.")
                    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientSecret), "DotBahn: 'ClientSecret' can't be null or empty.")
                    .ValidateOnStart();

            services.AddHttpClient();
            services.AddSingleton<IAuthorizationProvider>(sp => {
                var options = sp.GetRequiredService<IOptions<ModuleOptions>>().Value;

                return options.ProviderType switch {
                    AuthProviderType.ApiKey => new ApiKeyAuthorizationProvider(options),
                    _ => new NullAuthorizationProvider()
                };
            });
        
            return services;
        }
    }
}