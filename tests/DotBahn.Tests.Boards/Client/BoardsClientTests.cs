using System.Diagnostics;
using System.Net;
using DotBahn.Boards;
using DotBahn.Common.Auth;
using DotBahn.Common.Telemetry;
using DotBahn.Tests.Boards.Support;
using NSubstitute;
using Shouldly;

namespace DotBahn.Tests.Boards.Client;

public class BoardsClientTests {
    [Fact]
    public async Task GetArrivalsAsync_Should_ThrowArgumentNullException_When_QueryIsNull() {
        using var fixture = new BoardsClientFixture();

        var exception = await Should.ThrowAsync<ArgumentNullException>(() => fixture.Client.GetArrivalsAsync(null!));

        exception.ParamName.ShouldBe("query");
    }

    [Fact]
    public async Task GetDeparturesAsync_Should_ThrowArgumentNullException_When_QueryIsNull() {
        using var fixture = new BoardsClientFixture();

        var exception = await Should.ThrowAsync<ArgumentNullException>(() => fixture.Client.GetDeparturesAsync(null!));

        exception.ParamName.ShouldBe("query");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetArrivalsAsync_Should_PostToArrivalsEndpoint_When_Called(bool departures) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");

        if (departures) {
            await fixture.Client.GetDeparturesAsync(new BoardQuery(), TestContext.Current.CancellationToken);
        } else {
            await fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken);
        }

        var request = fixture.Handler.SentRequests.ShouldHaveSingleItem();
        request.Method.ShouldBe(HttpMethod.Post);
        request.RequestUri!.ToString().ShouldBe(departures ? "https://api.example.com/departures" : "https://api.example.com/arrivals");
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 400)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    public async Task GetArrivalsAsync_Should_ThrowHttpRequestException_When_ApiReturnsError(HttpStatusCode statusCode, int expectedCode) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(statusCode, "error");

        var exception = await Should.ThrowAsync<HttpRequestException>(() => fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken));

        exception.StatusCode.ShouldBe(statusCode);
        exception.Message.ShouldContain(expectedCode.ToString());
    }

    [Fact]
    public async Task GetDeparturesAsync_Should_ThrowOperationCanceledException_When_Canceled() {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() => fixture.Client.GetDeparturesAsync(new BoardQuery(), cts.Token));
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("not json")]
    [InlineData("null")]
    public async Task GetArrivalsAsync_Should_ThrowJsonException_When_ResponseIsInvalid(string json) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        await Should.ThrowAsync<System.Text.Json.JsonException>(() => fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_ReportBoardsApiTag_When_ListenerIsActive() {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == DotBahnTelemetry.SourceName;
        listener.Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded;
        listener.ActivityStarted = activities.Add;
        ActivitySource.AddActivityListener(listener);

        await fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken);

        activities.ShouldContain(a => (string?)a.GetTagItem(DotBahnTelemetry.ApiTag) == "boards");
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_AuthorizeAndSetAcceptHeader_When_Called() {
        var authorization = Substitute.For<IAuthorization>();
        using var fixture = new BoardsClientFixture(authorization);
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");

        await fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken);

        authorization.Received(1).AuthorizeRequest(Arg.Any<HttpRequestMessage>());
        fixture.Handler.SentRequests.ShouldHaveSingleItem().Headers.Accept.ShouldContain(header => header.MediaType == "application/json");
    }
}
