using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Boards.Models.Journeys;
using DotBahn.Boards.Models.Messages;
using DotBahn.Boards.Models.Operators;
using DotBahn.Common.Models;
using Shouldly;

namespace DotBahn.Tests.Boards.Models;

public class ModelDefaultsTests {
    [Fact]
    public void Board_Should_UseEmptyCollections_When_NotProvided() {
        var arrival = new Board<ArrivalEntry>();
        var departure = new Board<DepartureEntry>();

        arrival.ConsideredStopPlaces.ShouldBeEmpty();
        arrival.Entries.ShouldBeEmpty();
        arrival.StationMessages.ShouldBeEmpty();
        arrival.TruncatedTransportTypes.ShouldBeEmpty();
        departure.Entries.ShouldBeEmpty();
    }

    [Fact]
    public void Entries_Should_UseEmptyCollections_When_NotProvided() {
        var arrival = CreateArrivalEntry();
        var departure = CreateDepartureEntry();

        arrival.Messages.ShouldBeEmpty();
        arrival.ReliefBy.ShouldBeEmpty();
        arrival.ReliefFor.ShouldBeEmpty();
        arrival.ReplacedBy.ShouldBeEmpty();
        arrival.ReplacementFor.ShouldBeEmpty();
        arrival.TravelsWith.ShouldBeEmpty();
        arrival.ContinuationBy.ShouldBeEmpty();
        departure.Messages.ShouldBeEmpty();
        departure.ContinuationFor.ShouldBeEmpty();
    }

    [Fact]
    public void SupportingModels_Should_UseEmptyCollections_When_NotProvided() {
        var transport = new Transport {
            Category = "ICE",
            InternalCategory = "ICE",
            Type = "HIGH_SPEED_TRAIN",
            Number = "1",
            OperationalNumber = "1",
            Administration = new Administration { Id = "A", OperatorCode = "DB", OperatorName = "DB" }
        };
        var message = new Message { Text = "text", Type = MessageType.Note };
        var administration = new Administration { Id = "A", OperatorCode = "DB", OperatorName = "DB" };

        transport.Direction.ShouldBeEmpty();
        transport.DirectionStopPlaces.ShouldBeEmpty();
        transport.Via.ShouldBeEmpty();
        message.Resources.ShouldBeEmpty();
        administration.TransportCompanies.ShouldBeEmpty();
    }

    private static ArrivalEntry CreateArrivalEntry() => new() {
        Id = "a1",
        StopPlace = new StopPlace { EvaNumber = "1", Name = "Station" },
        Journey = CreateJourney(),
        Time = new ChangedValue<DateTime> { Original = DateTime.UnixEpoch },
        Platform = new ChangedRef<Platform> { Original = new Platform { Name = "1" } },
        Transport = CreateTransport()
    };

    private static DepartureEntry CreateDepartureEntry() => new() {
        Id = "d1",
        StopPlace = new StopPlace { EvaNumber = "1", Name = "Station" },
        Journey = CreateJourney(),
        Time = new ChangedValue<DateTime> { Original = DateTime.UnixEpoch },
        Platform = new ChangedRef<Platform> { Original = new Platform { Name = "1" } },
        Transport = CreateTransport()
    };

    private static Journey CreateJourney() => new() {
        Id = "j1",
        Origin = new Visit { StopPlace = new StopPlace { EvaNumber = "1", Name = "Station" } },
        Destination = new Visit { StopPlace = new StopPlace { EvaNumber = "2", Name = "Station 2" } },
        ScheduledStartTime = DateTime.UnixEpoch,
        TransportAtStart = CreateTransport(),
        Type = "REGULAR"
    };

    private static Transport CreateTransport() => new() {
        Category = "ICE",
        InternalCategory = "ICE",
        Type = "HIGH_SPEED_TRAIN",
        Number = "1",
        OperationalNumber = "1",
        Administration = new Administration { Id = "A", OperatorCode = "DB", OperatorName = "DB" }
    };
}
