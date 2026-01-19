using DotBahn.Modules.Authorization.Service;
using DotBahn.Modules.Authorization.Service.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        /// <param name="configuration">Delegate to configure <see cref="AuthorizationOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddDotBahnAuthorization(Action<AuthorizationOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            
            services.Configure(configuration);
            services.AddOptions<AuthorizationOptions>()
                    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "DotBahn: 'ClientId' can't be null or empty.")
                    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "DotBahn: 'ApiKey' can't be null or empty.")
                    .ValidateOnStart();
            
            services.TryAddSingleton<IAuthorization>(sp => { // Only add if not present
                var options = sp.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
                return new ApiKeyAuthorization(options);
            });
        
            return services;
        }
    }
}