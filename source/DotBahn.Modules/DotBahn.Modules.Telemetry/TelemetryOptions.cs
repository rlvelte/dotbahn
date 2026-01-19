namespace DotBahn.Modules.Telemetry;

/// <summary>
/// Configuration options for DotBahn telemetry.
/// </summary>
public class TelemetryOptions {
    /// <summary>
    /// Gets or sets whether HTTP instrumentation is enabled.
    /// </summary>
    public bool EnableHttpInstrumentation { get; set; }

    /// <summary>
    /// Gets or sets whether cache instrumentation is enabled.
    /// </summary>
    public bool EnableCacheInstrumentation { get; set; }
}
