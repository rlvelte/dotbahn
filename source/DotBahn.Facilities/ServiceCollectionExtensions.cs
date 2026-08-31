using DotBahn.Common.Auth;
using DotBahn.Common.Clients;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using DotBahn.Facilities.Internal.Contracts;
using DotBahn.Facilities.Internal.Transformers;
using DotBahn.Facilities.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Facilities;

/// <summary>
/// Extension methods for registering the DotBahn Facilities client
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Facilities";

    private static readonly Uri DefaultFacilitiesEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/");

    /// <summary>
    /// Registers the <see cref="IFacilityClient"/> with its parser and transformer services
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <param name="configuration">Optional action to override default client options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDotBahnFacilities(this IServiceCollection services, Action<ClientOptions>? configuration = null) {
        services.AddOptions<ClientOptions>(OptionsName)
                .Configure(opts => {
                    opts.BaseEndpoint = DefaultFacilitiesEndpoint;
                    configuration?.Invoke(opts);
                })
                .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseEndpoint must be an absolute URI.")
                .ValidateOnStart();

        services.AddHttpClient<IFacilityClient, FacilityClient>(OptionsName, (http, sp) => {
            var options = sp.GetRequiredService<IOptionsMonitor<ClientOptions>>().Get(OptionsName);
            http.BaseAddress = options.BaseEndpoint;
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/2.0 (+https://github.com/rlvelte/dotbahn)");

            var auth = sp.GetRequiredService<IAuthorization>();
            var parser = sp.GetRequiredService<IParser<IEnumerable<FacilityContract>>>();
            var transformer = sp.GetRequiredService<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>>();
            return new FacilityClient(http, auth, parser, transformer);
        })
        .AddStandardResilienceHandler();

        services.AddSingleton<IParser<IEnumerable<FacilityContract>>, JsonParser<List<FacilityContract>>>();
        services.AddSingleton<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>, FacilityTransformer>();

        return services;
    }
}
