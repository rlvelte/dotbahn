using System.Net;
using DotBahn.Tests.Timetables.Support;
using DotBahn.Timetables.Models;
using Shouldly;

namespace DotBahn.Tests.Timetables.Parsing;

public class TimetableXmlParserTests {
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetTimetableAsync_Should_ReturnEmptyTimetable_When_InputIsEmpty(string xml) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, xml, "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Station.ShouldBeEmpty();
        result.Stops.ShouldBeEmpty();
        result.Messages.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("""<other station="Hamburg"/>""")]
    [InlineData("""<timetable xmlns="urn:other" station="Hamburg"/>""")]
    public async Task GetTimetableAsync_Should_ReturnEmptyTimetable_When_RootIsNotTimetable(string xml) {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, xml, "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Station.ShouldBeEmpty();
        result.Stops.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetTimetableAsync_Should_ParseStationAndStops_When_AttributesProvided() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            <timetable station="Hamburg">
              <s id="stop-1" eva="8002549" />
              <s id="stop-2" />
            </timetable>
            """, "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Station.ShouldBe("Hamburg");
        var stops = result.Stops.ToList();
        stops.Count.ShouldBe(2);
        stops[0].Id.ShouldBe("stop-1");
        stops[1].Id.ShouldBe("stop-2");
    }

    [Fact]
    public async Task GetTimetableAsync_Should_ThrowXmlException_When_StopIdIsMissing() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "<timetable><s eva=\"1\"/></timetable>", "application/xml");

        var exception = await Should.ThrowAsync<System.Xml.XmlException>(() => fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken));

        exception.Message.ShouldContain("Missing required attribute 'id'");
    }

    [Fact]
    public async Task GetTimetableAsync_Should_SetMessagesNull_When_StopHasNoMessages() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "<timetable><s id=\"1\"/></timetable>", "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Messages.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetTimetableAsync_Should_ParseMessageAttributes_When_MessageProvided() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            <timetable><s id="1">
              <m id="m1" t="q" ts="2412311200" from="2412311100" to="2412311300"
                 c="42" int="1" del="1" ec="E" priority="2" owner="DB" cat="C">Bauarbeiten</m>
            </s></timetable>
            """, "application/xml");

        var result = await fixture.Client.GetTimetableAsync(1, DateTime.UnixEpoch, TestContext.Current.CancellationToken);

        var message = result.Stops.ShouldHaveSingleItem().Messages.ShouldHaveSingleItem();
        message.Id.ShouldBe("m1");
        message.Type.ShouldBe(DotBahn.Timetables.Models.Enumerations.MessageType.QualityChange);
        message.Timestamp.ShouldBe(new DateTime(2024, 12, 31, 12, 0, 0));
        message.ValidFrom.ShouldBe(new DateTime(2024, 12, 31, 11, 0, 0));
        message.ValidTo.ShouldBe(new DateTime(2024, 12, 31, 13, 0, 0));
        message.Code.ShouldBe(42);
        message.Priority.ShouldBe(DotBahn.Timetables.Models.Enumerations.MessagePriority.Medium);
        message.Owner.ShouldBe("DB");
        message.Category.ShouldBe("C");
        message.ExternalCategory.ShouldBe("E");
        message.InternalText.ShouldBe("Bauarbeiten");
        message.ExternalText.ShouldBeNull();
        message.IsDeleted.ShouldBeTrue();
    }
}
