using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DotBahn.Common.Telemetry;

/// <summary>
/// Central telemetry surface for DotBahn clients. Emits traces via <see cref="ActivitySource"/> and metrics via <see cref="Meter"/>.
/// </summary>
public static class DotBahnTelemetry {
    /// <summary>
    /// The name of the <see cref="ActivitySource"/> used for traces
    /// </summary>
    public const string SourceName = "DotBahn.Common";

    /// <summary>
    /// The name of the <see cref="Meter"/> used for metrics
    /// </summary>
    public const string MeterName = "DotBahn.Common";

    /// <summary>
    /// The name of the <see cref="ActivitySource"/> used for AI tool traces
    /// </summary>
    public const string AiSourceName = "DotBahn.Ai";

    /// <summary>
    /// The name of the <see cref="Meter"/> used for AI tool metrics
    /// </summary>
    public const string AiMeterName = "DotBahn.Ai";

    /// <summary>
    /// The name of the activity emitted for each client request
    /// </summary>
    public const string RequestActivityName = "dotbahn.request";

    /// <summary>
    /// The name of the activity emitted for each AI tool invocation
    /// </summary>
    public const string AiToolActivityName = "dotbahn.ai.tool";

    /// <summary>
    /// The name of the histogram measuring request duration in seconds
    /// </summary>
    public const string RequestDurationMetricName = "dotbahn.request.duration";

    /// <summary>
    /// The name of the counter measuring the number of requests
    /// </summary>
    public const string RequestCountMetricName = "dotbahn.request.count";

    /// <summary>
    /// The name of the histogram measuring AI tool duration in seconds
    /// </summary>
    public const string AiToolDurationMetricName = "dotbahn.ai.tool.duration";

    /// <summary>
    /// The name of the counter measuring the number of AI tool invocations
    /// </summary>
    public const string AiToolCountMetricName = "dotbahn.ai.tool.count";

    /// <summary>
    /// Tag identifying the API being accessed ('stations', 'timetables', 'facilities')
    /// </summary>
    public const string ApiTag = "dotbahn.api";

    /// <summary>
    /// Tag identifying the request endpoint including the query string
    /// </summary>
    public const string EndpointTag = "dotbahn.endpoint";

    /// <summary>
    /// Tag identifying the server host, following the stable HTTP semantic conventions
    /// </summary>
    public const string ServerAddressTag = "server.address";

    /// <summary>
    /// Tag identifying the AI tool being invoked
    /// </summary>
    public const string ToolTag = "dotbahn.tool";

    /// <summary>
    /// Tag identifying the error type, following the stable error semantic conventions
    /// </summary>
    public const string ErrorTypeTag = "error.type";

    /// <summary>
    /// Tag identifying the structured error code returned by an AI tool
    /// </summary>
    public const string ErrorCodeTag = "dotbahn.error_code";

    /// <summary>
    /// The version of the telemetry source, derived from the assembly version
    /// </summary>
    public static string Version { get; } = typeof(DotBahnTelemetry).Assembly.GetName().Version?.ToString() ?? "0.0.0";

    private static readonly ActivitySource Source = new(SourceName, Version);
    private static readonly Meter Meter = new(MeterName, Version);
    private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(RequestDurationMetricName, "s");
    private static readonly Counter<long> RequestCount = Meter.CreateCounter<long>(RequestCountMetricName, "{request}");
    private static readonly ActivitySource AiSource = new(AiSourceName, Version);
    private static readonly Meter AiMeter = new(AiMeterName, Version);
    private static readonly Histogram<double> AiToolDuration = AiMeter.CreateHistogram<double>(AiToolDurationMetricName, "s");
    private static readonly Counter<long> AiToolCount = AiMeter.CreateCounter<long>(AiToolCountMetricName, "{tool}");

    /// <summary>
    /// Starts a request activity. Returns <c>null</c> when no listener is registered, making telemetry a no-op.
    /// </summary>
    /// <param name="api">The API identifier, or <c>null</c> when unknown</param>
    /// <param name="endpoint">The request endpoint including the query string</param>
    /// <param name="serverAddress">The server host, or <c>null</c> when unknown</param>
    /// <returns>The started activity, or <c>null</c> when no listener is registered</returns>
    internal static Activity? StartRequest(string? api, string endpoint, string? serverAddress) {
        var activity = Source.StartActivity(RequestActivityName, ActivityKind.Client);
        if (activity == null) {
            return null;
        }

        if (api != null) {
            activity.SetTag(ApiTag, api);
        }

        if (serverAddress != null) {
            activity.SetTag(ServerAddressTag, serverAddress);
        }

        activity.SetTag(EndpointTag, endpoint);

        return activity;
    }

    /// <summary>
    /// Records the duration and count of a completed request
    /// </summary>
    /// <param name="durationSeconds">The request duration in seconds</param>
    /// <param name="api">The API identifier, or <c>null</c> when unknown</param>
    /// <param name="endpoint">The request endpoint including the query string</param>
    /// <param name="errorType">The exception type name on failure, or <c>null</c> on success</param>
    internal static void RecordRequest(double durationSeconds, string? api, string endpoint, string? errorType) {
        var tags = new TagList();

        if (api != null) {
            tags.Add(ApiTag, api);
        }

        if (errorType != null) {
            tags.Add(ErrorTypeTag, errorType);
        }

        tags.Add(EndpointTag, endpoint);

        RequestDuration.Record(durationSeconds, tags);
        RequestCount.Add(1, tags);
    }

    /// <summary>
    /// Starts an AI tool activity. Returns <c>null</c> when no listener is registered, making telemetry a no-op.
    /// </summary>
    /// <param name="name">The name of the tool being invoked</param>
    /// <returns>The started activity, or <c>null</c> when no listener is registered</returns>
    public static Activity? StartAiTool(string name) {
        var activity = AiSource.StartActivity(AiToolActivityName);
        if (activity == null) {
            return null;
        }

        activity.SetTag(ToolTag, name);
        return activity;
    }

    /// <summary>
    /// Records the duration and count of a completed AI tool invocation
    /// </summary>
    /// <param name="durationSeconds">The tool duration in seconds</param>
    /// <param name="name">The name of the tool being invoked</param>
    /// <param name="errorType">The exception type name on failure, or <c>null</c> on success</param>
    /// <param name="errorCode">The structured tool error code on failure, or <c>null</c> on success</param>
    public static void RecordAiTool(double durationSeconds, string name, string? errorType = null, string? errorCode = null) {
        var tags = new TagList {
            { ToolTag, name }
        };

        if (errorType != null) {
            tags.Add(ErrorTypeTag, errorType);
        }

        if (errorCode != null) {
            tags.Add(ErrorCodeTag, errorCode);
        }

        AiToolDuration.Record(durationSeconds, tags);
        AiToolCount.Add(1, tags);
    }
}
