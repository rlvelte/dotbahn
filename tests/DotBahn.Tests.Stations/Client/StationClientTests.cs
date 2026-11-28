using System.Diagnostics;
using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Common.Telemetry;
using DotBahn.Stations;
using DotBahn.Tests.Stations.Support;
using NSubstitute;
using Shouldly;

namespace DotBahn.Tests.Stations.Client;

public class StationClientTests {
    private const string EmptyResponse = """{"result":[]}""";

    [Fact]
    public async Task GetStationsAsync_Should_ThrowArgumentNullException_When_QueryIsNull() {
        using var fixture = new StationClientFixture();

        var exception = await Should.ThrowAsync<ArgumentNullException>(() => fixture.Client.GetStationsAsync(null!));

        exception.ParamName.ShouldBe("query");
    }

    [Fact]
    public async Task GetStationsAsync_Should_ReturnEmptyList_When_ResponseContainsNoStations() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetStationsAsync_Should_RequestStationsEndpoint_When_Called() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/stations?logicaloperator=AND&offset=0&limit=10000");
    }

    [Theory]
    [InlineData("7,1,4", "1,4,7")]
    [InlineData("1,1,7", "1,1,7")]
    [InlineData("3,2,1", "1,2,3")]
    public async Task GetStationsAsync_Should_SortByCategoryAscending_When_StationsAreUnsorted(string categories, string expected) {
        using var fixture = new StationClientFixture();
        var stations = string.Join(",", categories.Split(',').Select(category => $$"""{"number":1,"name":"S{{category}}","category":{{category}},"ril100Identifiers":[],"evaNumbers":[]}"""));
        fixture.Handler.RespondWith(HttpStatusCode.OK, $$"""{"result":[{{stations}}]}""");

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        result.Select(station => (int)station.Category).ShouldBe(categories.Split(',').Select(int.Parse).Order());
        result.Select(station => station.Name).ShouldBe(expected.Split(',').Select(name => $"S{name}"));
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 400)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    public async Task GetStationsAsync_Should_ThrowHttpRequestException_When_ApiReturnsError(HttpStatusCode statusCode, int expectedCode) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(statusCode, "error");

        var exception = await Should.ThrowAsync<HttpRequestException>(() => fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken));

        exception.StatusCode.ShouldBe(statusCode);
        exception.Message.ShouldContain(expectedCode.ToString());
    }

    [Fact]
    public async Task GetStationsAsync_Should_ThrowOperationCanceledException_When_Canceled() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() => fixture.Client.GetStationsAsync(new StationQuery(), cts.Token));
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("not json")]
    public async Task GetStationsAsync_Should_ThrowJsonException_When_ResponseIsInvalidJson(string json) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        await Should.ThrowAsync<System.Text.Json.JsonException>(() => fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetStationsAsync_Should_ReportStationsApiTag_When_ListenerIsActive() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == DotBahnTelemetry.SourceName;
        listener.Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded;
        listener.ActivityStarted = activities.Add;
        ActivitySource.AddActivityListener(listener);

        await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        activities.ShouldContain(a => (string?)a.GetTagItem(DotBahnTelemetry.ApiTag) == "stations");
    }

    [Fact]
    public async Task GetStationsAsync_Should_AuthorizeAndSetAcceptHeader_When_Called() {
        var authorization = Substitute.For<IAuthorization>();
        using var fixture = new StationClientFixture(authorization);
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        authorization.Received(1).AuthorizeRequest(Arg.Any<HttpRequestMessage>());
        fixture.Handler.SentRequests.ShouldHaveSingleItem().Headers.Accept.ShouldContain(header => header.MediaType == "application/json");
    }
}
