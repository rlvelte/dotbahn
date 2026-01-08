using DotBahn.Modules.Auth.Enumerations;
using DotBahn.Modules.Auth.Options;
using DotBahn.Modules.Auth.Service;
using DotBahn.Modules.Auth.Service.Base;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Modules.Auth;

/// <summary>
/// Extension methods for setting up authorization services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the authorization system, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="AuthOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddAuthorizationProvider(Action<IServiceProvider, AuthOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddSingleton<IConfigureOptions<AuthOptions>>(sp => new ConfigureOptions<AuthOptions>(opt => configuration(sp, opt)));
            services.AddOptions<AuthOptions>()
                    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "DotBahn: 'ClientId' can't be null or empty.")
                    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientSecret), "DotBahn: 'ClientSecret' can't be null or empty.")
                    .ValidateOnStart();

            services.AddHttpClient();
            services.AddSingleton<IAuthorizationProvider>(sp => {
                var options = sp.GetRequiredService<IOptions<AuthOptions>>().Value;

                return options.ProviderType switch {
                    AuthProviderType.ApiKey => new ApiKeyAuthorizationProvider(options),
                    _ => new NullAuthorizationProvider()
                };
            });
        
            return services;
        }
    }
}