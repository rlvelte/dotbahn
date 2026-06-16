namespace DotBahn.Samples.Shared;

/// <summary>
/// Helper for reading DotBahn API credentials from environment variables.
/// </summary>
public static class Credentials {
    /// <summary>
    /// Attempts to read DOTBAHN_CLIENT and DOTBAHN_SECRET from environment variables.
    /// </summary>
    public static bool TryFromEnvironment(out string clientId, out string clientSecret) {
        clientId = Environment.GetEnvironmentVariable("DOTBAHN_CLIENT") ?? string.Empty;
        clientSecret = Environment.GetEnvironmentVariable("DOTBAHN_SECRET") ?? string.Empty;
        return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret);
    }
}
