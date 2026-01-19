using System.Diagnostics;
using DotBahn.Modules.Telemetry.Additional;

namespace DotBahn.Modules.Telemetry.Service;

/// <summary>
/// DelegatingHandler that adds OpenTelemetry instrumentation to HTTP requests.
/// </summary>
public class TelemetryHttpMessageHandler : DelegatingHandler {
    /// <summary>
    /// Sends an HTTP request with telemetry instrumentation.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        if (!DotBahnDiagnostics.ActivitySource.HasListeners()) {
            return await base.SendAsync(request, cancellationToken);
        }

        using var activity = DotBahnDiagnostics.ActivitySource.StartActivity($"HTTP {request.Method}", ActivityKind.Client);
        if (activity == null) {
            return await base.SendAsync(request, cancellationToken);
        }

        activity.SetTag("http.method", request.Method.Method);
        activity.SetTag("http.url", request.RequestUri?.ToString());
        activity.SetTag("http.host", request.RequestUri?.Host);

        var stopwatch = Stopwatch.StartNew();
        try {
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            activity.SetTag("http.status_code", (int)response.StatusCode);
            if (!response.IsSuccessStatusCode) {
                activity.SetStatus(ActivityStatusCode.Error, $"HTTP {(int)response.StatusCode}");
            }

            DotBahnDiagnostics.HttpRequestCounter.Add(1,
                new KeyValuePair<string, object?>("http.method", request.Method.Method),
                new KeyValuePair<string, object?>("http.status_code", (int)response.StatusCode));

            DotBahnDiagnostics.HttpRequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("http.method", request.Method.Method),
                new KeyValuePair<string, object?>("http.status_code", (int)response.StatusCode));

            return response;
        } catch (Exception ex) {
            stopwatch.Stop();

            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.SetTag("error.type", ex.GetType().FullName);
            activity.SetTag("error.message", ex.Message);

            DotBahnDiagnostics.HttpRequestCounter.Add(1,
                new KeyValuePair<string, object?>("http.method", request.Method.Method),
                new KeyValuePair<string, object?>("error", true));

            DotBahnDiagnostics.HttpRequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("http.method", request.Method.Method),
                new KeyValuePair<string, object?>("error", true));

            throw;
        }
    }
}
