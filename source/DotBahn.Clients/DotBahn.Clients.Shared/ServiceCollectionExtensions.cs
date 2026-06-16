using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Clients.Shared;

/// <summary>
/// Shared helper for registering DotBahn typed clients.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers a typed DotBahn client with its interface and concrete implementation.
    /// </summary>
    /// <typeparam name="TClient">The client interface.</typeparam>
    /// <typeparam name="TImplementation">The concrete client implementation.</typeparam>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="optionsName">The named options name for this client.</param>
    /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnClient<TClient, TImplementation>(this IServiceCollection services, string optionsName, Action<ClientOptions> configuration)
        where TClient : class
        where TImplementation : ClientBase, TClient {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(optionsName);

        services.Configure(optionsName, configuration);
        services.AddOptions<ClientOptions>(optionsName)
                .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseEndpoint must be an absolute URI.")
                .ValidateOnStart();

        services.AddHttpClient<TClient, TImplementation>((sp, http) => {
            var opt = sp.GetRequiredService<IOptionsSnapshot<ClientOptions>>().Get(optionsName);
            http.BaseAddress = opt.BaseEndpoint;
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        services.AddTransient<TImplementation>(sp => (TImplementation)sp.GetRequiredService<TClient>());

        return services;
    }
}
