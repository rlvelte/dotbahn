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

    private static readonly Uri DefaultTimetablesEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1");

    /// <summary>
    /// Adds the Timetables client using HttpClientFactory.
    /// The <see cref="ClientOptions.BaseEndpoint"/> defaults to <c>https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1</c>
    /// and can be overridden in the optional delegate.
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Optional delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDotBahnTimetables(this IServiceCollection services, Action<ClientOptions>? configuration = null) {
        services.AddDotBahnClient<ITimetablesClient, TimetablesClient>(OptionsName, opts => {
            opts.BaseEndpoint = DefaultTimetablesEndpoint;
            configuration?.Invoke(opts);
        });

        services.AddSingleton<IParser<TimetableResponseContract>, XmlParser<TimetableResponseContract>>();
        services.AddSingleton<ITransformer<Timetable, TimetableResponseContract>, TimetableTransformer>();
        services.AddSingleton<IMerger<Timetable>, TimetableMerger>();

        return services;
    }
}
