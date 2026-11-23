using DotBahn.Boards.Internal.Contracts;
using DotBahn.Boards.Internal.Transformers;
using DotBahn.Boards.Models.Entries;
using DotBahn.Boards.Models.Messages;

namespace DotBahn.Boards.Tests.Transformer;

public class DepartureBoardTransformerTests {
    private readonly DepartureBoardTransformer _transformer = new();

    [Fact]
    public void Transform_NullContract_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => _transformer.Transform(null!));
    }

    [Fact]
    public void Transform_FullDeparture_MapsEntryAndJourneyFields() {
        var board = _transformer.Transform(Board([BoardContractFixtures.Departure()]));
        var entry = Assert.IsType<DepartureEntry>(Assert.Single(board.Entries));

        Assert.Equal("d-1", entry.Id);
        Assert.Equal(BoardContractFixtures.HannoverEva, entry.StopPlace.EvaNumber);
        Assert.Equal("Hannover Hbf", entry.StopPlace.Name);
        Assert.Equal("de:03241:8000105", entry.StopPlace.Dhid);
        Assert.Equal(BoardContractFixtures.HannoverStada, entry.StopPlace.StadaId);
        Assert.Equal("Hannover", entry.StopPlace.StationName);
        Assert.Equal("j-1", entry.Journey.Id);
        Assert.Equal("Wolfsburg Hbf", entry.Journey.Origin.StopPlace.Name);
        Assert.Equal("Hannover Hbf", entry.Journey.Destination.StopPlace.Name);
        Assert.True(entry.Journey.Destination.Cancelled);
        Assert.Equal("Wolfsburg Hbf", entry.Journey.DifferingOrigin?.Name);
        Assert.Equal("Hannover Hbf", entry.Journey.DifferingDestination?.Name);
        Assert.Equal(DateTime.Parse("2026-11-23T09:45:00+01:00"), entry.Journey.ScheduledStartTime);
        Assert.Equal("ICE", entry.Journey.TransportAtStart.Category);
        Assert.Equal("Deutsche Bahn", entry.Journey.HeaderAdministration?.OperatorName);
        Assert.Equal(701, entry.Journey.HeaderJourneyNumber);
        Assert.False(entry.Journey.IsCancelled);
        Assert.Equal("JOURNEY", entry.Journey.Type);
        Assert.False(entry.Cancelled);
        Assert.True(entry.Additional);
        Assert.False(entry.OnDemand);
        Assert.False(entry.NoPassengerChange);
        Assert.True(entry.VehicleSequenceAvailable);
        Assert.True(entry.FutureDisruptions);
    }

    [Fact]
    public void Transform_FullDeparture_MapsTransportAndLine() {
        var board = _transformer.Transform(Board([BoardContractFixtures.Departure()]));
        var entry = Assert.IsType<DepartureEntry>(Assert.Single(board.Entries));

        var transport = entry.Transport;
        Assert.Equal("ICE", transport.Category);
        Assert.Equal("ICE", transport.InternalCategory);
        Assert.Equal("HIGH_SPEED", transport.Type);
        Assert.Equal("701", transport.Number);
        Assert.Equal("701-2026-11-23", transport.OperationalNumber);
        Assert.Equal("Ice 701", transport.Description);
        Assert.Equal("ICE 701", transport.Label);
        Assert.Equal("dfid-1", transport.Dfid);
        Assert.Equal("dtid-1", transport.Dtid);
        Assert.Equal("BUS", transport.ReplacementTransport);
        Assert.Equal("DB", transport.Administration.OperatorCode);

        var line = transport.Line!;
        Assert.Equal("ICE 701", line.Name);
        Assert.Equal("#000000", line.PrimaryColour);
        Assert.Equal("#FFFFFF", line.PrimaryTextColour);
        Assert.Equal("#111111", line.ChipDifferingBoundaryColour);
        Assert.Equal("#222222", line.ChipDifferingFillColour);
        Assert.Equal("#333333", line.ChipDifferingTextColour);
        Assert.Equal("#444444", line.ChipSecondaryFillColour);
        Assert.Equal("#555555", line.StrokeDifferingBoundaryColour);
        Assert.Equal("#666666", line.StrokeSecondaryFillColour);

        Assert.Equal(["Wolfsburg Hbf"], transport.Direction);
        Assert.Equal([BoardContractFixtures.WolfsburgEva], transport.DirectionStopPlaces.Select(s => s.EvaNumber));
        var via = Assert.Single(transport.Via);
        Assert.Equal("Wolfsburg Hbf", via.StopPlace.Name);
        Assert.True(via.Additional);
        Assert.Equal(7, via.DisplayPriority);
    }

    [Fact]
    public void Transform_FullDeparture_MapsTransportReferences() {
        var board = _transformer.Transform(Board([BoardContractFixtures.Departure()]));
        var entry = Assert.IsType<DepartureEntry>(Assert.Single(board.Entries));

        var continuationFor = Assert.Single(entry.ContinuationFor);
        Assert.Equal("ref-1", continuationFor.JourneyId);
        Assert.Equal("RB", continuationFor.Category);
        Assert.Equal("4", continuationFor.Number);
        Assert.Equal("Wolfsburg Hbf", continuationFor.Origin?.StopPlace.Name);
        Assert.Equal("Hannover Hbf", continuationFor.DifferingOrigin?.Name);

        Assert.Single(entry.ReliefBy);
        Assert.Single(entry.ReliefFor);
        Assert.Single(entry.ReplacedBy);
        Assert.Single(entry.ReplacementFor);
        var travelsWith = Assert.Single(entry.TravelsWith);
        Assert.Equal("ref-3", travelsWith.JourneyId);
        Assert.Equal("Hannover Hbf", travelsWith.SeparationAt?.Name);

        var codeshare = Assert.Single(entry.Codeshares);
        Assert.Equal(1234, codeshare.Value);
        Assert.Equal("LH", codeshare.Key);
    }

    [Fact]
    public void Transform_FullDeparture_MapsResolvedMessages() {
        var board = _transformer.Transform(Board([BoardContractFixtures.Departure()]));
        var entry = Assert.IsType<DepartureEntry>(Assert.Single(board.Entries));

        Assert.Equal(5, entry.Messages.Count);
        Assert.Contains(entry.Messages, m => m.Type == MessageType.Attribute && m.Code == "HELP" && m.Text == "Barrierefreier Einstieg" && m.Priority == 1 && m.DetailedPriority == 2);
        Assert.Contains(entry.Messages, m => m.Type == MessageType.Disruption && m.Cause == "Construction" && m.Effect == "Delay" && m.HasAlternatives);
        Assert.Contains(entry.Messages, m => m.Type == MessageType.Note && m.Category == "GENERAL" && m.Code == "BIKE");
        Assert.Contains(entry.Messages, m => m.Type == MessageType.RisCauseCode && m.Code == "1");
        Assert.Contains(entry.Messages, m => m.Type == MessageType.RisQualityDeviation && m.Code == "Q1");

        var disruption = Assert.Single(entry.Messages, m => m.Type == MessageType.Disruption);
        Assert.Equal("Bauarh.", disruption.ShortText);
        Assert.Equal("D-1", disruption.DisruptionId);
        Assert.Equal("DC-1", disruption.DisruptionCommunicationId);
        Assert.Equal(MessageResourceType.Link, disruption.Resources[0].Type);
        Assert.Equal(MessageResourceType.Attachment, disruption.Resources[1].Type);
        Assert.Equal(MessageResourceType.Image, disruption.Resources[2].Type);
    }

    [Fact]
    public void Transform_MapsBoardLevelFields() {
        var board = _transformer.Transform(Board([BoardContractFixtures.Departure()], truncated: ["HIGH_SPEED"]));

        Assert.Equal(2, board.ConsideredStopPlaces.Count);
        Assert.Equal("Hannover Hbf", board.ConsideredStopPlaces[0].Name);
        Assert.Equal(["HIGH_SPEED"], board.TruncatedTransportTypes);
        var stationMessage = Assert.Single(board.StationMessages);
        Assert.Equal(MessageType.Disruption, stationMessage.Type);
    }

    [Theory]
    [InlineData("SCHEDULE", "2026-11-23T10:00:00+01:00", "2026-11-23T10:00:00+01:00", false)]
    [InlineData("SCHEDULE", "2026-11-23T10:00:00+01:00", "2026-11-23T10:07:00+01:00", true)]
    [InlineData("PREVIEW", "2026-11-23T10:00:00+01:00", "2026-11-23T10:00:00+01:00", true)]
    [InlineData("REAL", "2026-11-23T10:00:00+01:00", "2026-11-23T10:00:00+01:00", true)]
    public void Transform_TimeType_DeterminesUpdate(string timeType, string schedule, string actual, bool expectedHasUpdate) {
        var departure = BoardContractFixtures.Departure() with {
            TimeSchedule = schedule,
            Time = actual,
            TimeType = timeType
        };

        var entry = Assert.IsType<DepartureEntry>(Assert.Single(_transformer.Transform(Board([departure])).Entries));

        Assert.Equal(DateTime.Parse(schedule), entry.Time.Original);
        Assert.Equal(expectedHasUpdate, entry.Time.HasUpdate);
        Assert.Equal(expectedHasUpdate ? DateTime.Parse(actual) : null, entry.Time.Updated);
    }

    [Fact]
    public void Transform_PlatformChange_PreservesScheduledAndActual() {
        var departure = BoardContractFixtures.Departure() with {
            PlatformSchedule = new() { Name = "2", Accessibility = new() { StepFreeAccess = true } },
            Platform = new() { Name = "5", Accessibility = new() { BoardingAid = true } }
        };

        var entry = Assert.IsType<DepartureEntry>(Assert.Single(_transformer.Transform(Board([departure])).Entries));

        Assert.Equal("2", entry.Platform.Original.Name);
        Assert.True(entry.Platform.Original.StepFreeAccess);
        var updated = entry.Platform.Updated!;
        Assert.Equal("5", updated.Name);
        Assert.True(updated.BoardingAid);
    }

    [Fact]
    public void Transform_PlatformUnchanged_UpdatesToNull() {
        var platform = new PlatformContract { Name = "2" };
        var departure = BoardContractFixtures.Departure() with {
            PlatformSchedule = platform,
            Platform = new() { Name = "2" }
        };

        var entry = Assert.IsType<DepartureEntry>(Assert.Single(_transformer.Transform(Board([departure])).Entries));

        Assert.Equal("2", entry.Platform.Original.Name);
        Assert.Null(entry.Platform.Updated);
    }

    [Fact]
    public void Transform_MissingPlatforms_FallsBackToEmptyName() {
        var departure = BoardContractFixtures.Departure() with {
            PlatformSchedule = null,
            Platform = null
        };

        var entry = Assert.IsType<DepartureEntry>(Assert.Single(_transformer.Transform(Board([departure])).Entries));

        Assert.Equal(string.Empty, entry.Platform.Original.Name);
        Assert.Null(entry.Platform.Updated);
    }

    [Fact]
    public void Transform_LocalizedNames_PrefersGermanThenEnglishThenFirst() {
        var departure = BoardContractFixtures.Departure();
        var response = new DepartureBoardResponseContract {
            StopPlaces = [
                new() { EvaNumber = "1", LocalizedNames = [new() { Language = "eng", Name = "English" }, new() { Language = "deu", Name = "German" }] },
                new() { EvaNumber = "2", LocalizedNames = [new() { Language = "eng", Name = "English" }, new() { Language = "ita", Name = "Italian" }] },
                new() { EvaNumber = "3", LocalizedNames = [new() { Language = "ita", Name = "Italian" }, new() { Language = "fra", Name = "French" }] }
            ],
            Stations = [],
            Administrations = [],
            Messages = null,
            Meta = BoardContractFixtures.Meta(considered: ["1", "2", "3"]),
            Departures = [departure]
        };

        var board = _transformer.Transform(response);

        Assert.Equal(["German", "English", "Italian"], board.ConsideredStopPlaces.Select(s => s.Name));
    }

    [Fact]
    public void Transform_UnknownEvaNumber_FallsBackToEvaAsName() {
        var departure = BoardContractFixtures.Departure(eva: BoardContractFixtures.UnknownEva);

        var entry = Assert.IsType<DepartureEntry>(Assert.Single(_transformer.Transform(Board([departure])).Entries));

        Assert.Equal(BoardContractFixtures.UnknownEva, entry.StopPlace.EvaNumber);
        Assert.Equal(BoardContractFixtures.UnknownEva, entry.StopPlace.Name);
    }

    [Fact]
    public void Transform_UnknownAdministration_FallsBackToEmptyOperatorFields() {
        var transport = BoardContractFixtures.Transport() with { AdministrationId = "unknown-operator" };
        var departure = BoardContractFixtures.Departure() with { Transport = transport };

        var entry = Assert.IsType<DepartureEntry>(Assert.Single(_transformer.Transform(Board([departure])).Entries));

        Assert.Equal("unknown-operator", entry.Transport.Administration.Id);
        Assert.Equal(string.Empty, entry.Transport.Administration.OperatorCode);
        Assert.Equal(string.Empty, entry.Transport.Administration.OperatorName);
    }

    [Fact]
    public void Transform_ConsidersOnlyMetaStopPlaces() {
        var board = _transformer.Transform(Board([BoardContractFixtures.Departure()], considered: [BoardContractFixtures.HannoverEva, BoardContractFixtures.UnknownEva]));

        var considered = Assert.Single(board.ConsideredStopPlaces);
        Assert.Equal(BoardContractFixtures.HannoverEva, considered.EvaNumber);
    }

    [Fact]
    public void Transform_WithoutOptionalData_MapsEmptyCollections() {
        var departure = BoardContractFixtures.Departure() with {
            Messages = null,
            Codeshares = null,
            ContinuationFor = null,
            ReliefBy = null,
            ReliefFor = null,
            ReplacedBy = null,
            ReplacementFor = null,
            TravelsWith = null
        };
        var response = new DepartureBoardResponseContract {
            StopPlaces = BoardContractFixtures.StopPlaces(),
            Stations = BoardContractFixtures.Stations(),
            Administrations = BoardContractFixtures.Administrations(),
            Messages = null,
            Meta = BoardContractFixtures.Meta(),
            DisruptionCommunications = null,
            Departures = [departure]
        };

        var board = _transformer.Transform(response);
        var entry = Assert.IsType<DepartureEntry>(Assert.Single(board.Entries));

        Assert.Empty(entry.Messages);
        Assert.Empty(entry.Codeshares);
        Assert.Empty(entry.ContinuationFor);
        Assert.Empty(entry.ReliefBy);
        Assert.Empty(entry.ReliefFor);
        Assert.Empty(entry.ReplacedBy);
        Assert.Empty(entry.ReplacementFor);
        Assert.Empty(entry.TravelsWith);
        Assert.Empty(board.StationMessages);
    }

    private static DepartureBoardResponseContract Board(List<DepartureContract>? departures = null, List<string>? considered = null, List<string>? truncated = null) => new() {
        StopPlaces = BoardContractFixtures.StopPlaces(),
        Stations = BoardContractFixtures.Stations(),
        Administrations = BoardContractFixtures.Administrations(),
        Messages = BoardContractFixtures.Messages(),
        Meta = BoardContractFixtures.Meta(considered, truncated),
        DisruptionCommunications = [102],
        Departures = departures
    };
}
