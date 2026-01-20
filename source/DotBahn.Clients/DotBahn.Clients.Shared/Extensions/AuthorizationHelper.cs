using DotBahn.Modules.Authorization;
using DotBahn.Modules.Authorization.Service;
using DotBahn.Modules.Authorization.Service.Base;
using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Clients.Shared.Extensions;

/// <summary>
/// Helper for autoconfiguring authorization when adding clients.
/// </summary>
public static class AuthorizationHelper {
    private static string? _registeredClientId;
    private static string? _registeredApiKey;
    private static readonly Lock Lock = new();
    
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services) {
        /// <summary>
        /// Ensures there us an authorization instance presents when using a client.
        /// </summary>
        /// <param name="clientId">The Client ID for authentication.</param>
        /// <param name="apiKey">The api key for authentication.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void EnsureAuthorization(string? clientId, string? apiKey) {
            var hasCredentials = !string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(apiKey);
            lock (Lock) {
                if (!hasCredentials) {
                    return;
                }

                if (_registeredClientId != null && _registeredApiKey != null) {
                    if (_registeredClientId != clientId || _registeredApiKey != apiKey) {
                        throw new InvalidOperationException("DotBahn: Conflicting authorization credentials detected. Remove credentials from subsequent client registrations.");
                    }
                    
                    return;
                }
                    
                _registeredClientId = clientId;
                _registeredApiKey = apiKey;
                
                services.AddSingleton<IAuthorization>(_ => new ApiKeyAuthorization(new AuthorizationOptions {
                    ClientId = clientId!,
                    ApiKey = apiKey!
                }));
            }
        }
    }

    /// <summary>
    /// Resets the authorization state.
    /// </summary>
    internal static void Reset() {
        lock (Lock) {
            _registeredClientId = null;
            _registeredApiKey = null;
        }
    }
}
