using DotBahn.Common.Clients;
using Shouldly;

namespace DotBahn.Tests.Common.Clients;

public class ClientOptionsTests {
    [Fact]
    public void BaseEndpoint_Should_Be_Settable_And_Gettable_When_Initialized() {
        var expected = new Uri("https://example.com/");
        var sut = new ClientOptions {
            BaseEndpoint = expected
        };

        sut.BaseEndpoint.ShouldBe(expected);
    }
}
