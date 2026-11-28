using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotBahn.Ai;

/// <summary>
/// Extension methods for registering the DotBahn AI tools.
/// </summary>
public static class ServiceCollectionExtensions {
    private static readonly JsonSerializerOptions FunctionJsonOptions = CreateFunctionJsonOptions();

    /// <summary>
    /// Adds function invocation middleware with safe defaults for DotBahn tools.
    /// </summary>
    /// <param name="builder">The chat client builder to configure.</param>
    /// <param name="factory">Optional logger factory for function invocation logging.</param>
    /// <returns>The chat client builder for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <c>null</c>.</exception>
    public static ChatClientBuilder UseDotBahnAiTools(this ChatClientBuilder builder, ILoggerFactory? factory = null) {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.UseFunctionInvocation(factory, configure: client => {
            client.MaximumIterationsPerRequest = 5;
            client.MaximumConsecutiveErrorsPerRequest = 2;
            client.AllowConcurrentInvocation = false;
            client.IncludeDetailedErrors = false;
            client.TerminateOnUnknownCalls = true;
        });
    }

    /// <summary>
    /// Registers the DotBahn AI tools and their <see cref="AIFunction"/> adapters.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddDotBahnAiTools(this IServiceCollection services) {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<Tools>();

        services.AddSingleton(CreateSearchStationsFunction);
        services.AddSingleton(CreateDeparturesFunction);
        services.AddSingleton(CreateArrivalsFunction);
        services.AddSingleton(CreateFacilityStatusFunction);

        services.AddSingleton<IReadOnlyList<AIFunction>>(sp => sp.GetServices<AIFunction>().ToList());
        return services;
    }

    private static AIFunction CreateSearchStationsFunction(IServiceProvider services) {
        var tools = services.GetRequiredService<Tools>();
        return AIFunctionFactory.Create(tools.SearchStationsAsync, "dotbahn_search_stations", "Search for railway stations by name.", FunctionJsonOptions);
    }

    private static AIFunction CreateDeparturesFunction(IServiceProvider services) {
        var tools = services.GetRequiredService<Tools>();
        return AIFunctionFactory.Create(tools.GetDeparturesAsync, "dotbahn_get_departures", "Get scheduled departures for a station.", FunctionJsonOptions);
    }

    private static AIFunction CreateArrivalsFunction(IServiceProvider services) {
        var tools = services.GetRequiredService<Tools>();
        return AIFunctionFactory.Create(tools.GetArrivalsAsync, "dotbahn_get_arrivals", "Get scheduled arrivals for a station.", FunctionJsonOptions);
    }

    private static AIFunction CreateFacilityStatusFunction(IServiceProvider services) {
        var tools = services.GetRequiredService<Tools>();
        return AIFunctionFactory.Create(tools.GetFacilityStatusAsync, "dotbahn_get_facility_status", "Get operational status for station facilities.", FunctionJsonOptions);
    }

    private static JsonSerializerOptions CreateFunctionJsonOptions() {
        return new JsonSerializerOptions(AIJsonUtilities.DefaultOptions) {
            TypeInfoResolver = AIJsonUtilities.DefaultOptions.TypeInfoResolver is { } resolver ? JsonTypeInfoResolver.Combine(AiJsonContext.Default, resolver) : AiJsonContext.Default
        };
    }
}
