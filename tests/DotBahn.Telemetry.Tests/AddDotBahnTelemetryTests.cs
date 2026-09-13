using System.Diagnostics;
using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Common.Parsing;
using DotBahn.Common.Telemetry;
using DotBahn.Common.Tests;
using DotBahn.Common.Transformer;
using DotBahn.Stations;
using DotBahn.Stations.Internal.Contracts;
using DotBahn.Stations.Models;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using Moq;

namespace DotBahn.Telemetry.Tests;

[Collection("Telemetry")]
public class AddDotBahnTelemetryTests {
    [Fact]
    public async Task AddDotBahnTelemetry_RegistersSourceWithOpenTelemetry() {
        var activities = new List<Activity>();
        var services = new ServiceCollection();

        services.AddOpenTelemetry().WithTracing(tracing => tracing.AddInMemoryExporter(activities));
        services.AddDotBahnTelemetry();

        await using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<TracerProvider>();

        var parserMock = new Mock<IParser<StationsResponseContract>>();
        parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(new StationsResponseContract());
        var transformerMock = new Mock<ITransformer<IEnumerable<Station>, StationsResponseContract>>();
        transformerMock.Setup(t => t.Transform(It.IsAny<StationsResponseContract>())).Returns([]);

        var handler = new MockHttpHandler();
        handler.RespondWith(HttpStatusCode.OK, "{}", "application/json");
        using var client = new StationClient(new HttpClient(handler) {
            BaseAddress = new Uri("https://apis.deutschebahn.com/")
        }, Mock.Of<IAuthorization>(), parserMock.Object, transformerMock.Object);

        await client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        var exported = Assert.Single(activities, activity => activity.Source.Name == DotBahnTelemetry.SourceName);
        Assert.Equal(DotBahnTelemetry.RequestActivityName, exported.OperationName);
        Assert.Equal("stations", exported.GetTagItem(DotBahnTelemetry.ApiTag));
    }
}
