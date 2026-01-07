using System.Net;
using DotBahn.TimetableApi.Client;
using DotBahn.TimetableApi.Configuration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace DotBahn.TimetableApi;

/// <summary>
/// Extension methods for setting up Timetable API services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Adds the Timetable API client using HttpClientFactory, with options configured via callback.
        /// </summary>
        /// <param name="configure">Delegate to configure <see cref="TimetableConfiguration"/>. Can use the service provider.</param>
        /// <returns>The service collection.</returns>
        [UsedImplicitly]
        public IServiceCollection AddDotBahnTimetableApi(Action<IServiceProvider, TimetableConfiguration> configure) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);
            
            services.AddSingleton<IConfigureOptions<TimetableConfiguration>>(sp => new ConfigureOptions<TimetableConfiguration>(opt => configure(sp, opt)));
            services.AddOptions<TimetableConfiguration>()
                    .Validate(o => o.BaseEndpoint.IsAbsoluteUri, "DotBahn: BaseUri must be an absolute URI.")
                    .ValidateOnStart();

            services.AddHttpClient<TimetableApiClient>((sp, http) => {
                var options = sp.GetRequiredService<IOptions<TimetableConfiguration>>().Value;
                http.BaseAddress = options.BaseEndpoint;
                http.DefaultRequestHeaders.UserAgent.ParseAdd("DotBahn/1.0 (+https://github.com/rlvelte/dotbahn)");
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler((_, _) => BuildResiliencePolicy());
            
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
