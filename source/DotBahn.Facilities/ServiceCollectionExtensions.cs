using DotBahn.Facilities.Internal.Contracts;
using DotBahn.Facilities.Internal.Transformers;
using DotBahn.Facilities.Models;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using DotBahn.Shared;
using DotBahn.Shared.Parsing;
using DotBahn.Shared.Transformer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotBahn.Facilities;

/// <summary>
/// Extension methods for setting up FaSta in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string OptionsName = "DotBahn.Facilities";

    private static readonly Uri DefaultFacilitiesEndpoint = new("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/");

    /// <summary>
    /// Adds the FaSta client using HttpClientFactory.
    /// <remarks>
    /// The <see cref="ClientOptions.BaseEndpoint"/> defaults to <c>https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/</c> and can be overridden in the optional delegate.
    /// </remarks>
    /// </summary>
    /// <param name="services">The service collection to add this service to.</param>
    /// <param name="configuration">Optional delegate to configure <see cref="ClientOptions"/>. Can use the service provider.</param>
    /// <returns>The service collection.</returns>
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
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");

            var auth = sp.GetRequiredService<IAuthorization>();
            var cache = sp.GetService<ICache>();
            var parser = sp.GetRequiredService<IParser<IEnumerable<FacilityContract>>>();
            var transformer = sp.GetRequiredService<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>>();
            return new FacilityClient(http, auth, parser, transformer, cache);
        });

        services.AddSingleton<IParser<IEnumerable<FacilityContract>>, JsonParser<List<FacilityContract>>>();
        services.AddSingleton<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>, FacilityTransformer>();

        return services;
    }
}
