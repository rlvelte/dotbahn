using DotBahn.Modules.Cache.Service;
using DotBahn.Modules.Cache.Service.Base;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotBahn.Modules.Cache;

/// <summary>
/// Extension methods for setting up cache services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the cache system, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="CacheOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddCacheProvider(Action<IServiceProvider, CacheOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddSingleton<IConfigureOptions<CacheOptions>>(sp => new ConfigureOptions<CacheOptions>(opt => configuration(sp, opt)));
            services.AddOptions<CacheOptions>()
                    .Validate(o => o.DefaultExpiration.TotalSeconds > 1, "DotBahn: Cache 'DefaultExpiration' must be > 1.")
                    .ValidateOnStart();

            services.AddMemoryCache();
            services.AddSingleton<ICache>(sp => {
                var options = sp.GetRequiredService<IOptions<CacheOptions>>().Value;
                var cache = sp.GetRequiredService<IMemoryCache>();
                var logger = sp.GetService<ILogger<InMemoryCache>>();
                return new InMemoryCache(cache, options, logger);
            });
        
            return services;
        }
    }
}