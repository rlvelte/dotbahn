using System.Net;
using DotBahn.Common.Models;
using DotBahn.Tests.Timetables.Support;
using DotBahn.Timetables.Models;
using DotBahn.Timetables.Models.Enumerations;
using Shouldly;

namespace DotBahn.Tests.Timetables.Merge;

public class TimetableMergerTests {
    [Fact]
    public async Task GetFullChangesAsync_Should_AddNewStop_When_MergeOnProvided() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """<timetable station="New"><s id="new-stop"/></timetable>""", "application/xml");
        var current = new Timetable {
            Station = "Old"
        };

        var result = await fixture.Client.GetFullChangesAsync(1, current, TestContext.Current.CancellationToken);

        result.Station.ShouldBe("Old");
        result.Stops.ShouldHaveSingleItem().Id.ShouldBe("new-stop");
    }

    [Fact]
    public async Task GetFullChangesAsync_Should_MergeExistingStop_When_SameIdProvided() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            <timetable><s id="1">
              <dp pt="2412311200" cp="5" cs="c"/>
            </s></timetable>
            """, "application/xml");
        var current = new Timetable {
            Station = "Old",
            Stops = [Stop("1", departure: Event(platform: "3"))]
        };

        var result = await fixture.Client.GetFullChangesAsync(1, current, TestContext.Current.CancellationToken);

        var stop = result.Stops.ShouldHaveSingleItem();
        stop.Id.ShouldBe("1");
        stop.Departure!.Platform.Original.ShouldBe("3");
        stop.Departure.Platform.Updated.ShouldBe("5");
        stop.Departure.Status.Updated!.Value.ShouldBe(EventStatus.Cancelled);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetFullChangesAsync_Should_MergeEvent_When_OnlyOneSideHasEvent(bool currentHasEvent) {
        using var fixture = new TimetableClientFixture();
        var xml = currentHasEvent ? """<timetable><s id="1"/></timetable>""" : """<timetable><s id="1"><dp pt="2412311200"/></s></timetable>""";
        fixture.Handler.RespondWith(HttpStatusCode.OK, xml, "application/xml");
        var current = new Timetable {
            Station = "Old",
            Stops = [Stop("1", departure: currentHasEvent ? Event() : null)]
        };

        var result = await fixture.Client.GetFullChangesAsync(1, current, TestContext.Current.CancellationToken);

        if (currentHasEvent) {
            result.Stops.ShouldHaveSingleItem().Departure.ShouldBeNull();
        } else {
            result.Stops.ShouldHaveSingleItem().Departure.ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task GetFullChangesAsync_Should_KeepCurrentWings_When_ChangeHasNone() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """<timetable><s id="1"><dp pt="2412311200"/></s></timetable>""", "application/xml");
        var current = new Timetable {
            Station = "Old",
            Stops = [Stop("1", departure: Event(wings: ["W1", "W2"]))]
        };

        var result = await fixture.Client.GetFullChangesAsync(1, current, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Departure!.Wings.ShouldBe(["W1", "W2"]);
    }

    [Fact]
    public async Task GetRecentChangesAsync_Should_AppendMessages_WithoutDuplicates() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            <timetable><s id="1">
              <m id="m1"/><m id="m2"/>
            </s></timetable>
            """, "application/xml");
        var current = new Timetable {
            Station = "Old",
            Stops = [Stop("1", messages: [Message("m1"), Message("old")])]
        };

        var result = await fixture.Client.GetRecentChangesAsync(1, current, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Messages.Select(m => m.Id).ShouldBe(["m1", "old", "m2"]);
    }

    [Fact]
    public async Task GetFullChangesAsync_Should_IgnoreUpdatesEqualToOriginal_When_MergingValues() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK,
            """<timetable><s id="1"><dp pt="2412311200" pp="3"/></s></timetable>""", "application/xml");
        var current = new Timetable {
            Station = "Old",
            Stops = [Stop("1", departure: Event(platform: "3", time: new DateTime(2024, 12, 31, 12, 0, 0)))]
        };

        var result = await fixture.Client.GetFullChangesAsync(1, current, TestContext.Current.CancellationToken);

        var departure = result.Stops.ShouldHaveSingleItem().Departure!;
        departure.Platform.HasUpdate.ShouldBeFalse();
        departure.Time.HasUpdate.ShouldBeFalse();
    }

    [Fact]
    public async Task GetFullChangesAsync_Should_UseLastCurrentStop_When_DuplicateIdsExist() {
        using var fixture = new TimetableClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """<timetable><s id="1"><dp pt="2412311200" pp="9"/></s></timetable>""", "application/xml");
        var current = new Timetable {
            Station = "Old",
            Stops = [Stop("1", departure: Event(platform: "first")), Stop("1", departure: Event(platform: "last"))]
        };

        var result = await fixture.Client.GetFullChangesAsync(1, current, TestContext.Current.CancellationToken);

        result.Stops.ShouldHaveSingleItem().Departure!.Platform.Original.ShouldBe("last");
    }

    private static TimetableStop Stop(string id, TrainEvent? arrival = null, TrainEvent? departure = null, TimetableMessage[]? messages = null) => new() {
        Id = id,
        Train = new TrainLabel { Category = "ICE", Number = "1", Owner = "DB" },
        Arrival = arrival,
        Departure = departure,
        Messages = messages ?? []
    };

    private static TrainEvent Event(DateTime? time = null, string platform = "3", string[]? wings = null) => new() {
        Time = new ChangedValue<DateTime> { Original = time ?? new DateTime(2024, 1, 1, 12, 0, 0) },
        Platform = new ChangedRef<string> { Original = platform },
        Status = new ChangedValue<EventStatus> { Original = EventStatus.Planned },
        DistantEndpoint = new ChangedRef<string> { Original = string.Empty },
        Path = new ChangedRef<IEnumerable<string>> { Original = [] },
        Wings = wings ?? []
    };

    private static TimetableMessage Message(string id) => new() {
        Id = id,
        Type = MessageType.Him,
        Timestamp = new DateTime(2024, 1, 1)
    };
}
