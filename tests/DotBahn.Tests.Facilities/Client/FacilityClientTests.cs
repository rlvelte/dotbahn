using System.Diagnostics;
using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Common.Telemetry;
using DotBahn.Facilities;
using DotBahn.Tests.Facilities.Support;
using NSubstitute;
using Shouldly;

namespace DotBahn.Tests.Facilities.Client;

public class FacilityClientTests {
    private const string EmptyResponse = "[]";

    [Fact]
    public async Task GetFacilitiesAsync_Should_ThrowArgumentNullException_When_QueryIsNull() {
        using var fixture = new FacilityClientFixture();

        var exception = await Should.ThrowAsync<ArgumentNullException>(() => fixture.Client.GetFacilitiesAsync(null!));

        exception.ParamName.ShouldBe("query");
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_ReturnEmptyList_When_ResponseContainsNoFacilities() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_RequestFacilitiesEndpoint_When_Called() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/facilities");
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 400)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    public async Task GetFacilitiesAsync_Should_ThrowHttpRequestException_When_ApiReturnsError(HttpStatusCode statusCode, int expectedCode) {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(statusCode, "error");

        var exception = await Should.ThrowAsync<HttpRequestException>(() => fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken));

        exception.StatusCode.ShouldBe(statusCode);
        exception.Message.ShouldContain(expectedCode.ToString());
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_ThrowOperationCanceledException_When_Canceled() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() => fixture.Client.GetFacilitiesAsync(new FacilityQuery(), cts.Token));
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("not json")]
    [InlineData("null")]
    public async Task GetFacilitiesAsync_Should_ThrowJsonException_When_ResponseIsInvalid(string json) {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        await Should.ThrowAsync<System.Text.Json.JsonException>(() => fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_ReportFacilitiesApiTag_When_ListenerIsActive() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == DotBahnTelemetry.SourceName;
        listener.Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded;
        listener.ActivityStarted = activities.Add;
        ActivitySource.AddActivityListener(listener);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        activities.ShouldContain(a => (string?)a.GetTagItem(DotBahnTelemetry.ApiTag) == "facilities");
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_AuthorizeAndSetAcceptHeader_When_Called() {
        var authorization = Substitute.For<IAuthorization>();
        using var fixture = new FacilityClientFixture(authorization);
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        authorization.Received(1).AuthorizeRequest(Arg.Any<HttpRequestMessage>());
        fixture.Handler.SentRequests.ShouldHaveSingleItem().Headers.Accept.ShouldContain(header => header.MediaType == "application/json");
    }
}
