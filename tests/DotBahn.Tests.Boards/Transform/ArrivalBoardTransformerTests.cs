using System.Globalization;
using System.Net;
using DotBahn.Boards;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Boards.Models.Messages;
using DotBahn.Tests.Boards.Support;
using Shouldly;

namespace DotBahn.Tests.Boards.Transform;

public class ArrivalBoardTransformerTests {
    private const string BaseResponse = """
        {
          "stopPlaces":[{"evaNumber":"8002549","stada":"8002549","ifopt":"IF","localizedNames":[
            {"language":"eng","name":"English Station","symbol":"ES","symbolName":"Eng Symbol"},
            {"language":"deu","name":"Deutscher Bahnhof","symbol":"DS","symbolName":"Deu Symbol"}]}],
          "stations":[{"stada":"8002549","localizedNames":[
            {"language":"eng","name":"English Parent"},{"language":"deu","name":"Deutscher Parent"}]}],
          "administrations":[{"administrationId":"A1","operatorCode":"DB","operatorName":"Deutsche Bahn","transportCompanies":[
            {"language":"deu","name":"DB Fernverkehr","shortName":"DBF","website":"https://db.de","email":"a@b.de","phoneNumber":"123","websiteTicketing":"https://t","websiteDigitalAssistant":"https://d"}]}],
          "messages":{
            "attributes":[{"messageID":1,"code":"C1","displayPriority":1,"displayPriorityDetail":2,"descriptions":[
              {"language":"eng","text":"Attribute EN"},{"language":"deu","text":"Attribut DE"}]}],
            "disruptions":[{"messageID":2,"disruptionID":"D1","disruptionCommunicationID":"DC1","cause":"Bau","effect":"Verspaetung","hasAlternatives":true,"descriptions":[
              {"language":"deu","text":"Stoerung","textShort":"Kurz","links":[{"label":"Link","url":"https://link"}],"attachments":[{"label":"Att","url":"https://att"}],"images":[{"label":"Img","url":"https://img"}]}]}],
            "notes":[{"messageID":3,"category":"CAT","code":"N1","displayPriority":3,"descriptions":[{"language":"deu","text":"Notiz"}]}],
            "risCauseCodes":[{"messageID":4,"code":"R1","descriptions":[{"language":"deu","text":"Ursache"}]}],
            "risQualityDeviations":[{"messageID":5,"code":"Q1","descriptions":[{"language":"deu","text":"Qualitaet"}]}]
          },
          "meta":{"consideredStopPlaces":["8002549","missing"],"truncatedTransportTypes":["ICE"]},
          "disruptionCommunications":[2,999],
          "arrivals":[ENTRIES]
        }
        """;

    private const string DefaultEntry = """
        {"arrivalId":"a1","evaNumber":"8002549","journeyId":"j1",
         "journeyInfo":{"scheduledStartTime":"2024-12-31T10:00:00+01:00","origin":{"evaNumber":"8002549"},"destination":{"evaNumber":"missing"},"type":"REGULAR","headerAdministrationId":"A1","headerJourneyNumber":42,"transportAtStart":{"administrationId":"A1","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":123}},
         "timeSchedule":"2024-12-31T12:00:00+01:00","time":"2024-12-31T12:05:00+01:00","timeType":"REAL",
         "platformSchedule":{"name":"1"},"platform":{"name":"2"},
         "transport":{"administrationId":"A1","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":123,"operationalJourneyNumber":"456","journeyDescription":"ICE 123","replacementTransport":{"realType":"BUS"}},
         "messages":[1,2,999],"cancelled":true,"additional":true,"onDemand":true,"noPassengerChange":true,"vehicleSequenceAvailable":true,"pastDisruptions":true,
         "codeshares":[{"airlineCode":"LH","flightnumber":1234}]}
        """;

    [Fact]
    public async Task GetArrivalsAsync_Should_UseGermanLocalization_When_GermanNameExists() {
        var board = await GetBoardAsync(DefaultEntry);

        var stopPlace = board.Entries.ShouldHaveSingleItem().StopPlace;
        stopPlace.Name.ShouldBe("Deutscher Bahnhof");
        stopPlace.Symbol.ShouldBe("DS");
        stopPlace.SymbolName.ShouldBe("Deu Symbol");
        stopPlace.StationName.ShouldBe("Deutscher Parent");
        board.ConsideredStopPlaces.ShouldHaveSingleItem().Name.ShouldBe("Deutscher Bahnhof");
        board.TruncatedTransportTypes.ShouldBe(["ICE"]);
    }

    [Theory]
    [InlineData("REAL", true)]
    [InlineData("PREVIEW", true)]
    [InlineData("OTHER", false)]
    [InlineData("null", false)]
    public async Task GetArrivalsAsync_Should_SetTimeUpdate_When_TimeTypeOrTimesDiffer(string timeType, bool hasUpdate) {
        var attribute = timeType == "null" ? string.Empty : $",\"timeType\":\"{timeType}\"";
        var entry = DefaultEntry.Replace("\"timeType\":\"REAL\"", $"\"timeType\":\"{timeType}\"").Replace("\"time\":\"2024-12-31T12:05:00+01:00\"", timeType == "OTHER" || timeType == "null" ? "\"time\":\"2024-12-31T12:00:00+01:00\"" : "\"time\":\"2024-12-31T12:05:00+01:00\"");
        if (timeType == "null") {
            entry = entry.Replace($",\"timeType\":\"null\"{attribute}", string.Empty).Replace(",\"timeType\":\"null\"", string.Empty);
        }

        var board = await GetBoardAsync(entry);

        var time = board.Entries.ShouldHaveSingleItem().Time;
        time.Original.ShouldBe(DateTimeOffset.Parse("2024-12-31T12:00:00+01:00", CultureInfo.InvariantCulture).LocalDateTime);
        time.HasUpdate.ShouldBe(hasUpdate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetArrivalsAsync_Should_SetPlatformUpdate_When_PlatformNamesDiffer(bool withPlatform) {
        var entry = withPlatform ? DefaultEntry : DefaultEntry.Replace(",\"platform\":{\"name\":\"2\"}", string.Empty);

        var board = await GetBoardAsync(entry);

        var platform = board.Entries.ShouldHaveSingleItem().Platform;
        platform.Original.Name.ShouldBe("1");
        platform.HasUpdate.ShouldBe(withPlatform);
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_ResolveMessagesAndResources_When_IdsProvided() {
        var board = await GetBoardAsync(DefaultEntry);

        var messages = board.Entries.ShouldHaveSingleItem().Messages;
        messages.Select(m => m.Type).ShouldBe([MessageType.Attribute, MessageType.Disruption]);
        messages[0].Text.ShouldBe("Attribut DE");
        messages[1].ShortText.ShouldBe("Kurz");
        messages[1].Resources.Select(r => r.Type).ShouldBe([MessageResourceType.Link, MessageResourceType.Attachment, MessageResourceType.Image]);
        board.StationMessages.ShouldHaveSingleItem().Text.ShouldBe("Stoerung");
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_MapTransportFlagsAndFallbacks_When_EntryIsComplete() {
        var board = await GetBoardAsync(DefaultEntry);

        var entry = board.Entries.ShouldHaveSingleItem();
        entry.Cancelled.ShouldBeTrue();
        entry.Additional.ShouldBeTrue();
        entry.OnDemand.ShouldBeTrue();
        entry.NoPassengerChange.ShouldBeTrue();
        entry.VehicleSequenceAvailable.ShouldBeTrue();
        entry.PastDisruptions.ShouldBeTrue();
        entry.Codeshares.ShouldContainKeyAndValue("LH", 1234);
        entry.Transport.Number.ShouldBe("123");
        entry.Transport.Administration.OperatorName.ShouldBe("Deutsche Bahn");
        entry.Transport.Administration.TransportCompanies.ShouldHaveSingleItem().ShortName.ShouldBe("DBF");
        entry.Transport.ReplacementTransport.ShouldBe("BUS");
        entry.Journey.HeaderAdministration?.OperatorCode.ShouldBe("DB");
        entry.Journey.HeaderJourneyNumber.ShouldBe(42);
        entry.Journey.Destination.StopPlace.Name.ShouldBe("missing");
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_UseFallbackAdministration_When_IdIsUnknown() {
        var entry = DefaultEntry.Replace("\"administrationId\":\"A1\"", "\"administrationId\":\"unknown\"");

        var board = await GetBoardAsync(entry);

        var administration = board.Entries.ShouldHaveSingleItem().Transport.Administration;
        administration.Id.ShouldBe("unknown");
        administration.OperatorCode.ShouldBeEmpty();
        administration.OperatorName.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_ReturnEmptyBoard_When_ResponseIsEmpty() {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"meta":{}}""");

        var board = await fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken);

        board.Entries.ShouldBeEmpty();
        board.ConsideredStopPlaces.ShouldBeEmpty();
        board.StationMessages.ShouldBeEmpty();
        board.TruncatedTransportTypes.ShouldBeEmpty();
    }

    private const string RichEntry = """
        {"arrivalId":"a1","evaNumber":"8002549","journeyId":"j1",
         "journeyInfo":{"scheduledStartTime":"2024-12-31T10:00:00+01:00","origin":{"evaNumber":"8002549","cancelled":true},"destination":{"evaNumber":"8002549"},"differingOrigin":{"evaNumber":"8002549"},"differingDestination":{"evaNumber":"8002549"},"headerAdministrationId":"A1","headerJourneyNumber":1,"journeyCancelled":true,"type":"EXTRA","transportAtStart":{"administrationId":"A1","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":1}},
         "timeSchedule":"2024-12-31T12:00:00+01:00","time":"2024-12-31T12:00:00+01:00","timeType":"OTHER",
         "platformSchedule":{"name":"1","ifopt":"IF","accessibility":{"stepFreeAccess":true,"boardingAid":true,"tactileGuidingStrips":true,"tactileHandrailLabel":true,"tactilePlatformAccess":true,"audibleSignalsAvailable":true,"automaticDoor":true,"passengerInformationDisplay":true,"platformSign":true,"stairsMarking":true,"standardPlatformHeight":true}},
         "platform":{"name":"1","accessibility":{"stepFreeAccess":false}},
         "transport":{"administrationId":"A1","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":1,"line":{"name":"RE14","colour":{"primaryColour":"#fff","primaryTextColour":"#000","chipVisualization":{"differingBoundaryColour":"#1","differingFillColour":"#2","differingTextColour":"#3","secondaryFillColour":"#4"},"strokeVisualization":{"differingBoundaryColour":"#5","secondaryFillColour":"#6"}}},"direction":{"texts":["A","B"],"stopPlaces":[{"evaNumber":"8002549"}]},"via":[{"evaNumber":"8002549","additional":true,"cancelled":true,"displayPriority":2}],"replacementTransport":{"realType":"BUS"}},
         "messages":[1]}
        """;

    [Fact]
    public async Task GetArrivalsAsync_Should_MapRichTransportLineAndAccessibility_When_Provided() {
        var board = await GetBoardAsync(RichEntry);

        var entry = board.Entries.ShouldHaveSingleItem();
        entry.Journey.IsCancelled.ShouldBeTrue();
        entry.Journey.Type.ShouldBe("EXTRA");
        entry.Journey.DifferingOrigin.ShouldNotBeNull();
        entry.Journey.DifferingDestination.ShouldNotBeNull();
        entry.Journey.Origin.Cancelled.ShouldBeTrue();
        entry.Transport.Line!.Name.ShouldBe("RE14");
        entry.Transport.Line.PrimaryColour.ShouldBe("#fff");
        entry.Transport.Line.ChipDifferingBoundaryColour.ShouldBe("#1");
        entry.Transport.Line.StrokeSecondaryFillColour.ShouldBe("#6");
        entry.Transport.Direction.ShouldBe(["A", "B"]);
        entry.Transport.DirectionStopPlaces.ShouldHaveSingleItem().Name.ShouldBe("Deutscher Bahnhof");
        entry.Transport.Via.ShouldHaveSingleItem().Additional.ShouldBeTrue();
        entry.Transport.Via.ShouldHaveSingleItem().DisplayPriority.ShouldBe(2);
        entry.Transport.ReplacementTransport.ShouldBe("BUS");
        entry.Platform.Original.Ifopt.ShouldBe("IF");
        entry.Platform.Original.StepFreeAccess.ShouldBeTrue();
        entry.Platform.Original.BoardingAid.ShouldBeTrue();
        entry.Platform.Original.StandardPlatformHeight.ShouldBeTrue();
        entry.Platform.HasUpdate.ShouldBeFalse();
    }

    private const string MinimalEntry = """
        {"arrivalId":"a1","evaNumber":"unknown","journeyId":"j1",
         "journeyInfo":{"scheduledStartTime":"2024-12-31T10:00:00+01:00","origin":{"evaNumber":""},"destination":{"evaNumber":""},"headerAdministrationId":"","transportAtStart":{"administrationId":"","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":1}},
         "timeSchedule":"2024-12-31T12:00:00+01:00","time":"2024-12-31T12:00:00+01:00",
         "transport":{"administrationId":"unknown","category":"ICE","categoryInternal":"ICE","type":"HIGH_SPEED_TRAIN","journeyNumber":1}}
        """;

    [Fact]
    public async Task GetArrivalsAsync_Should_UseDefaults_When_OptionalFieldsAreMissing() {
        var board = await GetBoardAsync(MinimalEntry);

        var arrival = board.Entries.ShouldHaveSingleItem();
        arrival.Cancelled.ShouldBeFalse();
        arrival.Additional.ShouldBeFalse();
        arrival.OnDemand.ShouldBeFalse();
        arrival.NoPassengerChange.ShouldBeFalse();
        arrival.VehicleSequenceAvailable.ShouldBeFalse();
        arrival.PastDisruptions.ShouldBeFalse();
        arrival.Messages.ShouldBeEmpty();
        arrival.Codeshares.ShouldBeEmpty();
        arrival.ReliefBy.ShouldBeEmpty();
        arrival.TravelsWith.ShouldBeEmpty();
        arrival.Time.HasUpdate.ShouldBeFalse();
        arrival.Platform.Original.Name.ShouldBeEmpty();
    }

    private static async Task<Board<ArrivalEntry>> GetBoardAsync(string entry) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, BaseResponse.Replace("ENTRIES", entry));

        return await fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken);
    }
}
