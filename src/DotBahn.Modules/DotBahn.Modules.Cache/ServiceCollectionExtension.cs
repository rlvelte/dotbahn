using DotBahn.Modules.Cache.Configuration;
using DotBahn.Modules.Cache.Enumerations;
using DotBahn.Modules.Cache.Service;
using DotBahn.Modules.Cache.Service.Base;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Modules.Cache;

/// <summary>
/// Extension methods for setting up cache services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Adds the cache system, with options configured via callback.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Delegate to configure <see cref="CacheConfiguration"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddCacheProvider(this IServiceCollection services, Action<IServiceProvider, CacheConfiguration> configuration) {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IConfigureOptions<CacheConfiguration>>(sp => new ConfigureOptions<CacheConfiguration>(opt => configuration(sp, opt)));
        services.AddOptions<CacheConfiguration>()
                .Validate(o => o.DefaultExpiration.TotalSeconds > 1, "DotBahn: Cache 'DefaultExpiration' must be > 1.")
                .ValidateOnStart();

        services.AddMemoryCache();
        services.AddSingleton<ICacheProvider>(sp => {
            var options = sp.GetRequiredService<IOptions<CacheConfiguration>>().Value;

            return options.ProviderType switch {
                CacheProviderType.InMemory => new InMemoryCacheProvider(sp.GetRequiredService<IMemoryCache>()),
                CacheProviderType.Sqlite => new SqliteCacheProvider(options.SqliteDatabasePath),
                _ => new NullCacheProvider()
            };
        });
        
        return services;
    }
}