using System.Diagnostics;
using System.Globalization;
using System.Net;
using DotBahn.Common.Auth;
using DotBahn.Common.Telemetry;
using DotBahn.Tests.Timetables.Support;
using DotBahn.Timetables;
using DotBahn.Timetables.Models;
using NSubstitute;
using Shouldly;

namespace DotBahn.Tests.Timetables.Client;

public class TimetableClientTests {
    private const string EmptyXml = "<timetable/>";

    [Theory]
    [InlineData(2024, 12, 31, 23, 59, "/plan/8002549/241231/23")]
    [InlineData(2000, 1, 1, 0, 0, "/plan/8002549/000101/00")]
    [InlineData(1999, 12, 31, 12, 30, "/plan/8002549/991231/12")]
    public async Task GetTimetableAsync_Should_RequestInvariantDateAndHour_When_Called(int year, int month, int day, int hour, int minute, string expectedPath) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyXml, "application/xml");
        var dateTime = new DateTime(year, month, day, hour, minute, 0);

        await fixture.Client.GetTimetableAsync(8002549, dateTime, TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe($"https://api.example.com{expectedPath}");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetFullChangesAsync_Should_UseChanges_When_MergeOnProvided(bool mergeOnProvided) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """<timetable station="New"/>""", "application/xml");
        var mergeOn = new Timetable { Station = "Old" };

        var result = await fixture.Client.GetFullChangesAsync(8002549, mergeOnProvided ? mergeOn : null, TestContext.Current.CancellationToken);

        result.Station.ShouldBe(mergeOnProvided ? "Old" : "New");
        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/fchg/8002549");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetRecentChangesAsync_Should_UseChanges_When_MergeOnProvided(bool mergeOnProvided) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """<timetable station="New"/>""", "application/xml");
        var mergeOn = new Timetable { Station = "Old" };

        var result = await fixture.Client.GetRecentChangesAsync(8002549, mergeOnProvided ? mergeOn : null, TestContext.Current.CancellationToken);

        result.Station.ShouldBe(mergeOnProvided ? "Old" : "New");
        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/rchg/8002549");
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 400)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    public async Task GetTimetableAsync_Should_ThrowHttpRequestException_When_ApiReturnsError(HttpStatusCode statusCode, int expectedCode) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(statusCode, "error", "application/xml");

        var exception = await Should.ThrowAsync<HttpRequestException>(() => fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken));

        exception.StatusCode.ShouldBe(statusCode);
        exception.Message.ShouldContain(expectedCode.ToString());
    }

    [Fact]
    public async Task GetTimetableAsync_Should_ThrowOperationCanceledException_When_Canceled() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyXml, "application/xml");
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() => fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, cts.Token));
    }

    [Theory]
    [InlineData("<timetable><s/></timetable>")]
    [InlineData("<timetable>")]
    public async Task GetTimetableAsync_Should_ThrowXmlException_When_XmlIsInvalid(string xml) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, xml, "application/xml");

        await Should.ThrowAsync<System.Xml.XmlException>(() => fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetTimetableAsync_Should_ReportTimetablesApiTag_When_ListenerIsActive() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyXml, "application/xml");
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == DotBahnTelemetry.SourceName;
        listener.Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded;
        listener.ActivityStarted = activities.Add;
        ActivitySource.AddActivityListener(listener);

        await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        activities.ShouldContain(a => (string?)a.GetTagItem(DotBahnTelemetry.ApiTag) == "timetables");
    }

    [Fact]
    public async Task GetTimetableAsync_Should_AuthorizeAndSetAcceptHeader_When_Called() {
        var authorization = Substitute.For<IAuthorization>();
        using var fixture = new TimetableClientFixture(authorization);
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyXml, "application/xml");

        await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        authorization.Received(1).AuthorizeRequest(Arg.Any<HttpRequestMessage>());
        fixture.Handler.SentRequests.ShouldHaveSingleItem().Headers.Accept.ShouldContain(header => header.MediaType == "application/xml");
    }
}
