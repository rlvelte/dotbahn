using DotBahn.Common.Auth;
using Shouldly;

namespace DotBahn.Tests.Common.Auth;

public class AuthorizationOptionsTests {
    [Theory]
    [InlineData("client", "key", "ClientId: set, ApiKey: set")]
    [InlineData(" ", "key", "ClientId: not set, ApiKey: set")]
    [InlineData("client", "\t", "ClientId: set, ApiKey: not set")]
    [InlineData("", "", "ClientId: not set, ApiKey: not set")]
    [InlineData("   ", "   ", "ClientId: not set, ApiKey: not set")]
    [InlineData("client", "", "ClientId: set, ApiKey: not set")]
    [InlineData("", "key", "ClientId: not set, ApiKey: set")]
    public void ToString_Should_ReflectWhichFieldsAreSet_When_ValuesAreProvided(string clientId, string apiKey, string expected) {
        var sut = new AuthorizationOptions {
            ClientId = clientId,
            ApiKey = apiKey
        };

        var result = sut.ToString();

        result.ShouldBe(expected);
    }

    [Fact]
    public void ToString_Should_IndicateBothSet_When_ValuesAreNonWhitespace() {
        var sut = new AuthorizationOptions {
            ClientId = "my-client",
            ApiKey = "my-api-key"
        };

        var result = sut.ToString();

        result.ShouldBe("ClientId: set, ApiKey: set");
    }
}
