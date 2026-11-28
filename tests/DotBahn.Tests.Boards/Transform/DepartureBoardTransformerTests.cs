using System.Net;
using DotBahn.Boards;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Tests.Boards.Support;
using Shouldly;

namespace DotBahn.Tests.Boards.Transform;

public class DepartureBoardTransformerTests {
    private const string BaseResponse = """
        {
          "stopPlaces":[{"evaNumber":"8002549","localizedNames":[{"language":"deu","name":"Hamburg Hbf"}]}],
          "meta":{},
          "departures":[ENTRY]
        }
        """;

    private const string DefaultEntry = """
        {"departureId":"d1","evaNumber":"8002549","journeyId":"j1",
         "journeyInfo":{"scheduledStartTime":"2024-12-31T10:00:00+01:00","origin":{"evaNumber":"8002549"},"destination":{"evaNumber":"8002549"},"headerAdministrationId":"","type":"REGULAR","transportAtStart":{"administrationId":"unknown","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":123}},
         "timeSchedule":"2024-12-31T12:00:00+01:00","time":"2024-12-31T12:05:00+01:00","timeType":"REAL",
         "platformSchedule":{"name":"1"},"platform":{"name":"2"},
         "transport":{"administrationId":"unknown","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":123},
         "futureDisruptions":true,"cancelled":true}
        """;

    [Fact]
    public async Task GetDeparturesAsync_Should_MapEntryAndJourney_When_ResponseIsComplete() {
        var board = await GetBoardAsync(DefaultEntry);

        var entry = board.Entries.ShouldHaveSingleItem();
        entry.Id.ShouldBe("d1");
        entry.StopPlace.Name.ShouldBe("Hamburg Hbf");
        entry.Journey.Id.ShouldBe("j1");
        entry.Journey.ScheduledStartTime.ShouldBe(new DateTime(2024, 12, 31, 10, 0, 0, DateTimeKind.Unspecified));
        entry.Journey.Type.ShouldBe("REGULAR");
        entry.Time.HasUpdate.ShouldBeTrue();
        entry.Platform.HasUpdate.ShouldBeTrue();
        entry.Cancelled.ShouldBeTrue();
        entry.FutureDisruptions.ShouldBeTrue();
    }

    [Fact]
    public async Task GetDeparturesAsync_Should_FallbackToEvaName_When_StopPlaceIsUnknown() {
        var entry = DefaultEntry.Replace("\"evaNumber\":\"8002549\"", "\"evaNumber\":\"unknown\"");

        var board = await GetBoardAsync(entry);

        board.Entries.ShouldHaveSingleItem().StopPlace.Name.ShouldBe("unknown");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetDeparturesAsync_Should_SetPlatformUpdate_When_NamesDiffer(bool withPlatform) {
        var entry = withPlatform ? DefaultEntry : DefaultEntry.Replace(",\"platform\":{\"name\":\"2\"}", string.Empty);

        var board = await GetBoardAsync(entry);

        board.Entries.ShouldHaveSingleItem().Platform.HasUpdate.ShouldBe(withPlatform);
    }

    [Fact]
    public async Task GetDeparturesAsync_Should_ReturnEmptyBoard_When_ResponseIsEmpty() {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"meta":{}}""");

        var board = await fixture.Client.GetDeparturesAsync(new BoardQuery(), TestContext.Current.CancellationToken);

        board.Entries.ShouldBeEmpty();
    }

    private const string MinimalEntry = """
        {"departureId":"d1","evaNumber":"unknown","journeyId":"j1",
         "journeyInfo":{"scheduledStartTime":"2024-12-31T10:00:00+01:00","origin":{"evaNumber":""},"destination":{"evaNumber":""},"headerAdministrationId":"","transportAtStart":{"administrationId":"","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":1}},
         "timeSchedule":"2024-12-31T12:00:00+01:00","time":"2024-12-31T12:00:00+01:00",
         "transport":{"administrationId":"unknown","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":1}}
        """;

    [Fact]
    public async Task GetDeparturesAsync_Should_UseDefaults_When_OptionalFieldsAreMissing() {
        var board = await GetBoardAsync(MinimalEntry);

        var departure = board.Entries.ShouldHaveSingleItem();
        departure.Cancelled.ShouldBeFalse();
        departure.FutureDisruptions.ShouldBeFalse();
        departure.Time.HasUpdate.ShouldBeFalse();
        departure.Platform.Original.Name.ShouldBeEmpty();
        departure.ContinuationFor.ShouldBeEmpty();
        departure.ReliefBy.ShouldBeEmpty();
    }

    private static async Task<Board<DepartureEntry>> GetBoardAsync(string entry) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, BaseResponse.Replace("ENTRY", entry));

        return await fixture.Client.GetDeparturesAsync(new BoardQuery(), TestContext.Current.CancellationToken);
    }
}
