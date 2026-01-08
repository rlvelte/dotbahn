namespace DotBahn.Modules.Auth.Enumerations;

/// <summary>
/// Supported authorization provider types
/// </summary>
public enum AuthProviderType {
    /// <summary>
    /// No authorization
    /// </summary>
    None,

    /// <summary>
    /// API Key-based authorization (Headers)
    /// </summary>
    ApiKey
}
