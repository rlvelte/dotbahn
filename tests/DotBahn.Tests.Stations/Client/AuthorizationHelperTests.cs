using System.Reflection;

using DotBahn.Clients.Shared.Extensions;
using DotBahn.Modules.Authorization.Service;
using DotBahn.Modules.Authorization.Service.Base;

using Microsoft.Extensions.DependencyInjection;

namespace DotBahn.Tests.Stations.Client;

/// <summary>
/// Tests for <see cref="AuthorizationHelper.EnsureAuthorization"/> extension method.
/// </summary>
public class AuthorizationHelperTests : IDisposable {
    private static readonly MethodInfo ResetMethod = typeof(AuthorizationHelper)
        .GetMethod("Reset", BindingFlags.Static | BindingFlags.NonPublic)!;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationHelperTests"/> class.
    /// Resets the static authorization state before each test via reflection.
    /// </summary>
    public AuthorizationHelperTests() {
        ResetMethod.Invoke(null, null);
    }

    /// <inheritdoc />
    public void Dispose() {
        ResetMethod.Invoke(null, null);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void EnsureAuthorization_WithValidCredentials_RegistersAuthorizationSingleton() {
        var services = new ServiceCollection();

        services.EnsureAuthorization("test-client-id", "test-api-key");

        var provider = services.BuildServiceProvider();
        var auth = provider.GetService<IAuthorization>();

        Assert.NotNull(auth);
        Assert.IsType<ApiKeyAuthorization>(auth);
    }

    [Fact]
    public void EnsureAuthorization_WithSameCredentials_DoesNotThrow() {
        var services = new ServiceCollection();
        services.EnsureAuthorization("test-client-id", "test-api-key");

        var exception = Record.Exception(
            () => services.EnsureAuthorization("test-client-id", "test-api-key"));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureAuthorization_WithDifferentCredentials_ThrowsInvalidOperationException() {
        var services = new ServiceCollection();
        services.EnsureAuthorization("test-client-id", "test-api-key");

        var ex = Assert.Throws<InvalidOperationException>(() => services.EnsureAuthorization("other-client-id", "other-api-key"));

        Assert.Contains("conflicting", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    public void EnsureAuthorization_WithNullOrEmptyCredentials_ReturnsSilently(string? clientId, string? apiKey) {
        var services = new ServiceCollection();

        var exception = Record.Exception(() => services.EnsureAuthorization(clientId, apiKey));

        Assert.Null(exception);
        Assert.Empty(services);
    }
}
