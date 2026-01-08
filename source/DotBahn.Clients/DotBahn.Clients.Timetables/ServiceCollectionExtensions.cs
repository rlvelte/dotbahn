using System.Net;
using DotBahn.Modules.Shared.Parsing;
using DotBahn.Modules.Shared.Transformer;
using DotBahn.Timetables.Client;
using DotBahn.Timetables.Contracts;
using DotBahn.Timetables.Models;
using DotBahn.Timetables.Options;
using DotBahn.Timetables.Transformers;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace DotBahn.Timetables;

/// <summary>
/// Extension methods for setting up Timetables services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the Timetables client using HttpClientFactory, with options configured via callback.
        /// </summary>
        /// <param name="configuration">Delegate to configure <see cref="TimetableOptions"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddDotBahnTimetables(Action<IServiceProvider, TimetableOptions> configuration) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
        
            services.AddSingleton<IConfigureOptions<TimetableOptions>>(sp => new ConfigureOptions<TimetableOptions>(opt => configuration(sp, opt)));
            services.AddOptions<TimetableOptions>()
                    .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseUri must be an absolute URI.")
                    .ValidateOnStart();

            services.AddHttpClient<TimetablesClient>((sp, http) => {
                var options = sp.GetRequiredService<IOptions<TimetableOptions>>().Value;
                http.BaseAddress = options.BaseEndpoint;
                http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler((_, _) => BuildResiliencePolicy());
            
            services.AddSingleton<ITransformer<EventContract, EventInfo?>, EventTransformer>();
            services.AddSingleton<ITransformer<MessageContract, Message>, MessageTransformer>();
            services.AddSingleton<ITransformer<StationContract, StationInfo>, StationTransformer>();
            services.AddSingleton<ITransformer<StopDataContract, TrainStop>, StopTransformer>();
            services.AddSingleton<ITransformer<TimetableResponseContract, Timetable>, TimetableTransformer>();
            
            services.AddSingleton<IParser<TimetableResponseContract>, XmlParser<TimetableResponseContract>>();
            services.AddSingleton<IParser<StationsResponseContract>, XmlParser<StationsResponseContract>>();
        
            return services;
        }
    }

    private static IAsyncPolicy<HttpResponseMessage> BuildResiliencePolicy() {
        var jitter = new Random();
        var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

        var retry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode is (HttpStatusCode)429 or HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(retryCount: 4, (retryAttempt, outcome, _) => {
                                   var retryAfter = outcome?.Result?.Headers?.RetryAfter;
                                   if (retryAfter != null) {
                                       var ra = retryAfter.Delta ?? (retryAfter.Date.HasValue ? retryAfter.Date.Value - DateTimeOffset.UtcNow : null);
                                       if (ra.HasValue && ra.Value > TimeSpan.Zero) {
                                           var capped = ra.Value > TimeSpan.FromSeconds(30) ? TimeSpan.FromSeconds(30) : ra.Value;
                                           return capped;
                                       }
                                   }

                                   var backoff = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                                   var jitterMs = jitter.Next(0, 250);
                                   var delay = backoff + TimeSpan.FromMilliseconds(jitterMs);

                                   return delay > TimeSpan.FromSeconds(20) ? TimeSpan.FromSeconds(20) : delay;
                               }, (_, _, _, _) => Task.CompletedTask
            );

        var circuitBreaker = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == (HttpStatusCode)429)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30)
            );

        return Policy.WrapAsync(retry, circuitBreaker, timeout);
    }
}
