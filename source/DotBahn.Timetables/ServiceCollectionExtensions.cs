using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using DotBahn.Timetables.Internal.Contracts;
using DotBahn.Timetables.Internal.Parsing;
using DotBahn.Timetables.Internal.Transformers;
using DotBahn.Timetables.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Timetables;

/// <summary>
/// Extension methods for registering the DotBahn Timetables client
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Timetables";

    private static readonly Uri DefaultTimetablesEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/timetables/v1/");

    /// <summary>
    /// Registers the <see cref="ITimetableClient"/> with its parser, transformer, and merger services
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <param name="configuration">Optional action to override default client options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDotBahnTimetables(this IServiceCollection services, Action<ClientOptions>? configuration = null) {
        services.AddOptions<ClientOptions>(OptionsName)
                .Configure(opts => {
                    opts.BaseEndpoint = DefaultTimetablesEndpoint;
                    configuration?.Invoke(opts);
                })
                .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseEndpoint must be an absolute URI.")
                .ValidateOnStart();

        services.AddHttpClient<ITimetableClient, TimetableClient>(OptionsName, (http, sp) => {
            var options = sp.GetRequiredService<IOptionsMonitor<ClientOptions>>().Get(OptionsName);
            http.BaseAddress = options.BaseEndpoint;
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/2.0 (+https://github.com/rlvelte/dotbahn)");

            var auth = sp.GetRequiredService<IAuthorization>();
            var parser = sp.GetRequiredService<IParser<TimetableResponseContract>>();
            var transformer = sp.GetRequiredService<ITransformer<Timetable, TimetableResponseContract>>();
            var merger = sp.GetRequiredService<IMerger<Timetable>>();
            return new TimetableClient(http, auth, parser, transformer, merger);
        })
        .AddStandardResilienceHandler();

        services.AddSingleton<IParser<TimetableResponseContract>, TimetableXmlParser>();
        services.AddSingleton<ITransformer<Timetable, TimetableResponseContract>, TimetableTransformer>();
        services.AddSingleton<IMerger<Timetable>, TimetableMerger>();

        return services;
    }
}
