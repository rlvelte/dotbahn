using DotBahn.Tests.Shared;
using Shouldly;

namespace DotBahn.Common.Tests.Clients;

public class ClientBaseIntegrationTests(HttpTestFixture fixture) : IClassFixture<HttpTestFixture> {

    [Fact]
    public void Fixture_Should_Provide_Client_And_Handler() {
        fixture.Client.ShouldNotBeNull();
        fixture.Handler.ShouldNotBeNull();
    }
}
