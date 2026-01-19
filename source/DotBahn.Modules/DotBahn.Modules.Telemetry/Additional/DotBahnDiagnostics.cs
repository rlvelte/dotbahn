using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DotBahn.Modules.Telemetry.Additional;

/// <summary>
/// Central telemetry definitions for DotBahn clients.
/// </summary>
public static class DotBahnDiagnostics {
    /// <summary>
    /// The name used for the ActivitySource and Meter.
    /// </summary>
    public const string SourceName = "DotBahn";

    /// <summary>
    /// ActivitySource for distributed tracing.
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(SourceName, "1.0.0");

    /// <summary>
    /// Meter for metrics collection.
    /// </summary>
    public static readonly Meter Meter = new(SourceName, "1.0.0");

    /// <summary>
    /// Counter for HTTP requests made.
    /// </summary>
    public static readonly Counter<long> HttpRequestCounter = Meter.CreateCounter<long>(
        "dotbahn.http.requests",
        unit: "{request}",
        description: "Number of HTTP requests made by DotBahn clients");

    /// <summary>
    /// Histogram for HTTP request duration.
    /// </summary>
    public static readonly Histogram<double> HttpRequestDuration = Meter.CreateHistogram<double>(
        "dotbahn.http.duration",
        unit: "ms",
        description: "Duration of HTTP requests in milliseconds");

    /// <summary>
    /// Counter for cache hits.
    /// </summary>
    public static readonly Counter<long> CacheHitCounter = Meter.CreateCounter<long>(
        "dotbahn.cache.hits",
        unit: "{hit}",
        description: "Number of cache hits");

    /// <summary>
    /// Counter for cache misses.
    /// </summary>
    public static readonly Counter<long> CacheMissCounter = Meter.CreateCounter<long>(
        "dotbahn.cache.misses",
        unit: "{miss}",
        description: "Number of cache misses");
}
