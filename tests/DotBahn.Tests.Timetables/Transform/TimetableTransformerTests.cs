using System.Net;
using DotBahn.Tests.Timetables.Support;
using DotBahn.Timetables.Models;
using DotBahn.Timetables.Models.Enumerations;
using Shouldly;

namespace DotBahn.Tests.Timetables.Transform;

public class TimetableTransformerTests {
    [Theory]
    [InlineData("2412311200", 2024, 12, 31, 12, 0)]
    [InlineData("0001010000", 2000, 1, 1, 0, 0)]
    [InlineData("9912312359", 1999, 12, 31, 23, 59)]
    [InlineData("", 1, 1, 1, 0, 0)]
    [InlineData("not-a-time", 1, 1, 1, 0, 0)]
    public async Task GetTimetableAsync_Should_ParsePlannedTime_When_ValueProvided(string time, int year, int month, int day, int hour, int minute) {
        var departure = await GetDepartureAsync($"""<dp pt="{time}"/>""");

        var expected = year == 1 ? default : new DateTime(year, month, day, hour, minute, 0);
        departure.Time.Original.ShouldBe(expected);
        departure.Time.HasUpdate.ShouldBeFalse();
    }

    [Theory]
    [InlineData("2412311300", 2024, 12, 31, 13)]
    [InlineData("bad", 2024, 12, 31, 12)]
    [InlineData("null", 2024, 12, 31, 12)]
    public async Task GetTimetableAsync_Should_ParseChangedTime_When_ValueProvided(string time, int year, int month, int day, int hour) {
        var attribute = time == "null" ? string.Empty : $" ct=\"{time}\"";
        var departure = await GetDepartureAsync($"""<dp pt="2412311200"{attribute}/>""");

        var hasUpdate = time != "bad" && time != "null";
        departure.Time.HasUpdate.ShouldBe(hasUpdate);
        if (hasUpdate) {
            departure.Time.Updated!.Value.ShouldBe(new DateTime(year, month, day, hour, 0, 0));
        }
    }

    [Theory]
    [InlineData("p", EventStatus.Planned)]
    [InlineData("a", EventStatus.Added)]
    [InlineData("c", EventStatus.Cancelled)]
    [InlineData("x", EventStatus.Unknown)]
    public async Task GetTimetableAsync_Should_ParsePlannedStatus_When_ValueProvided(string status, EventStatus expected) {
        var departure = await GetDepartureAsync($"""<dp ps="{status}"/>""");

        departure.Status.Original.ShouldBe(expected);
    }

    [Theory]
    [InlineData("c", true)]
    [InlineData("a", true)]
    [InlineData("x", false)]
    [InlineData("null", false)]
    public async Task GetTimetableAsync_Should_SetChangedStatus_When_ValueIsKnown(string status, bool hasUpdate) {
        var attribute = status == "null" ? string.Empty : $" cs=\"{status}\"";
        var departure = await GetDepartureAsync($"""<dp ps="p"{attribute}/>""");

        departure.Status.HasUpdate.ShouldBe(hasUpdate);
        if (hasUpdate) {
            departure.Status.Updated!.Value.ShouldNotBe(EventStatus.Unknown);
        }
    }

    [Theory]
    [InlineData("A|B|C", 3)]
    [InlineData("A||B", 2)]
    [InlineData("", 0)]
    [InlineData("null", 0)]
    public async Task GetTimetableAsync_Should_ParsePipeSeparatedPath_When_ValueProvided(string path, int expectedCount) {
        var attribute = path == "null" ? string.Empty : $" ppth=\"{path}\"";
        var departure = await GetDepartureAsync($"""<dp{attribute}/>""");

        departure.Path.Original.Count().ShouldBe(expectedCount);
        departure.Path.HasUpdate.ShouldBeFalse();
    }

    [Fact]
    public async Task GetTimetableAsync_Should_SetChangedPath_When_ChangedPathProvided() {
        var departure = await GetDepartureAsync("""<dp ppth="A|B" cpth="C|D"/>""");

        departure.Path.HasUpdate.ShouldBeTrue();
        departure.Path.Updated!.ShouldBe(new[] { "C", "D" });
    }

    [Theory]
    [InlineData("p", TripType.Passenger)]
    [InlineData("e", TripType.Empty)]
    [InlineData("z", TripType.Z)]
    [InlineData("s", TripType.S)]
    [InlineData("h", TripType.H)]
    [InlineData("n", TripType.N)]
    [InlineData("x", TripType.Passenger)]
    [InlineData("null", null)]
    public async Task GetTimetableAsync_Should_ParseTripType_When_ValueProvided(string type, TripType? expected) {
        using var fixture = new TimetableClientFixture();
        var attribute = type == "null" ? string.Empty : $" t=\"{type}\"";
        fixture.Handler.RespondWith(HttpStatusCode.OK, $"""<timetable><s id="1"><tl c="ICE" n="123" o="DB"{attribute}/></s></timetable>""", "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Train.Type.ShouldBe(expected);
    }

    [Fact]
    public async Task GetTimetableAsync_Should_PreferDepartureLine_When_BothEventsProvideLine() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            <timetable><s id="1">
              <ar l="AR-LINE"/><dp l="DP-LINE"/>
            </s></timetable>
            """, "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Train.Line.ShouldBe("DP-LINE");
    }

    [Fact]
    public async Task GetTimetableAsync_Should_FallBackToArrivalLine_When_DepartureHasNoLine() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            <timetable><s id="1">
              <ar l="AR-LINE"/><dp/>
            </s></timetable>
            """, "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Train.Line.ShouldBe("AR-LINE");
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("null", false)]
    public async Task GetTimetableAsync_Should_AssignMessageText_When_InternalFlagProvided(string internalFlag, bool isInternal) {
        using var fixture = new TimetableClientFixture();
        var attribute = internalFlag == "null" ? string.Empty : $" int=\"{internalFlag}\"";
        fixture.Handler.RespondWith(HttpStatusCode.OK, $"""<timetable><s id="1"><m id="m1"{attribute}>Text</m></s></timetable>""", "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        var message = result.Stops.ShouldHaveSingleItem().Messages.ShouldHaveSingleItem();
        message.InternalText.ShouldBe(isInternal ? "Text" : null);
        message.ExternalText.ShouldBe(isInternal ? null : "Text");
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData("abc", null)]
    [InlineData("null", null)]
    public async Task GetTimetableAsync_Should_ParseMessageCode_When_ValueProvided(string code, int? expected) {
        using var fixture = new TimetableClientFixture();
        var attribute = code == "null" ? string.Empty : $" c=\"{code}\"";
        fixture.Handler.RespondWith(HttpStatusCode.OK, $"""<timetable><s id="1"><m id="m1"{attribute}/></s></timetable>""", "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Messages.ShouldHaveSingleItem().Code.ShouldBe(expected);
    }

    [Fact]
    public async Task GetTimetableAsync_Should_UseDefaults_When_StopHasNoOptionalParts() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "<timetable><s id=\"1\"/></timetable>", "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        var stop = result.Stops.ShouldHaveSingleItem();
        stop.Arrival.ShouldBeNull();
        stop.Departure.ShouldBeNull();
        stop.Train.Category.ShouldBeEmpty();
        stop.Train.Number.ShouldBeEmpty();
        stop.Train.Owner.ShouldBeEmpty();
        stop.Train.Type.ShouldBeNull();
    }

    private static async Task<TrainEvent> GetDepartureAsync(string eventXml) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK,
            $"<timetable><s id=\"1\">{eventXml}</s></timetable>", "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        return result.Stops.ShouldHaveSingleItem().Departure!;
    }
}
