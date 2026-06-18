using DotBahn.Common.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DotBahn.Common;

/// <summary>
/// Shared helper for registering DotBahn typed clients.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers the DotBahn authorization services using the provided configuration.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="configuration">The action to configure authorization options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
    public static IServiceCollection AddDotBahnAuthorization(this IServiceCollection services, Action<AuthorizationOptions> configuration) {
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure(configuration);
        services.AddOptions<AuthorizationOptions>()
                .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "DotBahn: 'ClientId' can't be null or empty.")
                .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "DotBahn: 'ApiKey' can't be null or empty.")
                .ValidateOnStart();

        services.TryAddSingleton<IAuthorization>(sp => {
            var options = sp.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
            return new ApiKeyAuthorization(options);
        });

        return services;
    }
}
