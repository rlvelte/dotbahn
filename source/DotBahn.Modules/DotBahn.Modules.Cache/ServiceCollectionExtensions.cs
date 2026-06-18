using DotBahn.Modules.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotBahn.Modules.Cache;

/// <summary>
/// Extension methods for setting up cache services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Adds the cache system, with options configured via callback.
    /// </summary>
    /// <param name="configuration">Delegate to configure <see cref="CacheOptions"/>. Can use the service provider.</param>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The service collection.</returns>
    public static void AddDotBahnCache(this IServiceCollection services, Action<CacheOptions> configuration) {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure(configuration);
        services.AddOptions<CacheOptions>()
                .Validate(o => o.DefaultExpiration.TotalSeconds > 1, "DotBahn: Cache 'DefaultExpiration' must be > 1.")
                .ValidateOnStart();

        services.AddSingleton<ICache>(sp => {
            var options = sp.GetRequiredService<IOptions<CacheOptions>>().Value;
            var logger = sp.GetService<ILogger<InMemoryCache>>();
            return new InMemoryCache(options, logger);
        });
    }
}
