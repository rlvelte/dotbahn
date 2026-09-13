using DotBahn.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace DotBahn.Telemetry;

/// <summary>
/// Extension methods for registering the DotBahn telemetry options
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers the DotBahn <see cref="System.Diagnostics.ActivitySource"/> and
    /// <see cref="System.Diagnostics.Metrics.Meter"/> with the OpenTelemetry providers.
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c></exception>
    public static IServiceCollection AddDotBahnTelemetry(this IServiceCollection services) {
        ArgumentNullException.ThrowIfNull(services);

        services.ConfigureOpenTelemetryTracerProvider((_, builder) => builder.AddSource(DotBahnTelemetry.SourceName));
        services.ConfigureOpenTelemetryMeterProvider((_, builder) => builder.AddMeter(DotBahnTelemetry.MeterName));

        return services;
    }
}
