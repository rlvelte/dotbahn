using DotBahn.Clients.Shared;
using DotBahn.Clients.Shared.Parsing;
using DotBahn.Clients.Shared.Parsing.Base;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Interfaces;
using DotBahn.Clients.Timetables.Transformer;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Timetables.Models;

using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Clients.Timetables;

/// <summary>
/// Extension methods for setting up Timetables in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtension {
    private const string OptionsName = "DotBahn.Timetables";

    /// <summary>
    /// Adds the Timetables client using HttpClientFactory, with options configured via callback.
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnTimetables(this IServiceCollection services, Action<ClientOptions> configuration) {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDotBahnClient<ITimetablesClient, TimetablesClient>(OptionsName, configuration);

        services.AddSingleton<IParser<TimetableResponseContract>, XmlParser<TimetableResponseContract>>();
        services.AddSingleton<ITransformer<Timetable, TimetableResponseContract>, TimetableTransformer>();
        services.AddSingleton<IMerger<Timetable>, TimetableMerger>();

        return services;
    }
}
