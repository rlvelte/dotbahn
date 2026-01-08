using DotBahn.Modules.RequestCache.Enumerations;
using DotBahn.Modules.RequestCache.Service;
using DotBahn.Modules.RequestCache.Service.Base;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Modules.RequestCache;

/// <summary>
/// Extension methods for setting up request cache services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the request cache system, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="ModuleOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddRequestCacheProvider(Action<IServiceProvider, ModuleOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddSingleton<IConfigureOptions<ModuleOptions>>(sp => new ConfigureOptions<ModuleOptions>(opt => configuration(sp, opt)));
            services.AddOptions<ModuleOptions>()
                    .Validate(o => o.DefaultExpiration.TotalSeconds > 1, "DotBahn: Cache 'DefaultExpiration' must be > 1.")
                    .ValidateOnStart();

            services.AddMemoryCache();
            services.AddSingleton<IRequestCache>(sp => {
                var options = sp.GetRequiredService<IOptions<ModuleOptions>>().Value;

                return options.ProviderType switch {
                    CacheProviderType.InMemory => new RequestCacheProvider(sp.GetRequiredService<IMemoryCache>()),
                    _ => new NullProvider()
                };
            });
        
            return services;
        }
    }
}