using DotBahn.Boards.Internal.Utilities;

namespace DotBahn.Boards.Tests.Query;

public class QueryUtilTests {
    [Fact]
    public void ToArrivalContract_WithCompleteQuery_MapsAllFields() {
        var query = new BoardQuery {
            EvaNumbers = ["8000105", "8000096"],
            Platforms = ["2", "3"],
            BoardConfigurations = ["EXPANDED"],
            Start = new DateTime(2026, 11, 23, 10, 0, 0, DateTimeKind.Unspecified),
            End = new DateTime(2026, 11, 23, 12, 0, 0, DateTimeKind.Unspecified),
            TransportTypes = ["HIGH_SPEED", "REGIONAL"],
            Languages = ["deu"],
            MaxEntriesPerTransportType = 50,
            MaxViaStops = 5,
            NoCancellations = true,
            OnlyReplacementTransports = true,
            SortBy = "TIME_SCHEDULE",
            IncludeLineColours = true,
            IncludePlatformAccessibility = true
        };

        var request = QueryUtil.ToArrivalContract(query);
        var timeWindow = request.TimeWindow!;

        Assert.Equal(["8000105", "8000096"], request.StopPlaceOrStation.EvaNumbers);
        Assert.Equal(["2", "3"], request.StopPlaceOrStation.Platforms);
        Assert.Equal(["EXPANDED"], request.StopPlaceOrStation.BoardConfigurations);
        Assert.Equal("2026-11-23T10:00:00Z", timeWindow.TimeStart);
        Assert.Equal("2026-11-23T12:00:00Z", timeWindow.TimeEnd);
        Assert.Equal(["HIGH_SPEED", "REGIONAL"], request.TransportTypes);
        Assert.Equal(["deu"], request.Languages);
        Assert.Equal(50, request.MaxEntriesPerTransportType);
        Assert.Equal(5, request.MaxViaStops);
        Assert.True(request.NoCancellations);
        Assert.True(request.OnlyReplacementTransports);
        Assert.Equal("TIME_SCHEDULE", request.SortBy);
        Assert.True(request.IncludeLineColours);
        Assert.True(request.IncludePlatformAccessibility);
    }

    [Fact]
    public void ToDepartureContract_WithCompleteQuery_MapsAllFields() {
        var query = new BoardQuery {
            StadaId = "8000105",
            Dhids = ["de:03241:8000105"],
            Start = new DateTime(2026, 11, 23, 10, 0, 0, DateTimeKind.Unspecified),
            End = new DateTime(2026, 11, 23, 12, 0, 0, DateTimeKind.Unspecified),
            Languages = ["deu", "eng"],
            MaxViaStops = 2
        };

        var request = QueryUtil.ToDepartureContract(query);
        var timeWindow = request.TimeWindow!;

        Assert.Equal("8000105", request.StopPlaceOrStation.Stada);
        Assert.Equal(["de:03241:8000105"], request.StopPlaceOrStation.Dhids);
        Assert.Equal("2026-11-23T10:00:00Z", timeWindow.TimeStart);
        Assert.Equal("2026-11-23T12:00:00Z", timeWindow.TimeEnd);
        Assert.Equal(2, request.MaxViaStops);
    }

    [Fact]
    public void ToArrivalContract_WithDefaults_AppliesApiFallbacks() {
        var request = QueryUtil.ToArrivalContract(new BoardQuery());

        Assert.Null(request.StopPlaceOrStation.EvaNumbers);
        Assert.Null(request.StopPlaceOrStation.Stada);
        Assert.Null(request.StopPlaceOrStation.Dhids);
        Assert.Null(request.StopPlaceOrStation.Platforms);
        Assert.Null(request.StopPlaceOrStation.BoardConfigurations);
        Assert.Null(request.TimeWindow);
        Assert.Null(request.TransportTypes);
        Assert.Equal(["deu", "eng"], request.Languages);
        Assert.Equal(25, request.MaxEntriesPerTransportType);
        Assert.Equal(3, request.MaxViaStops);
        Assert.Equal("TIME", request.SortBy);
        Assert.False(request.IncludeLineColours);
        Assert.False(request.IncludePlatformAccessibility);
    }

    [Fact]
    public void ToArrivalContract_WithEmptyCollections_MapsToNull() {
        var query = new BoardQuery {
            EvaNumbers = [],
            Dhids = [],
            Platforms = [],
            BoardConfigurations = [],
            TransportTypes = []
        };

        var request = QueryUtil.ToArrivalContract(query);

        Assert.Null(request.StopPlaceOrStation.EvaNumbers);
        Assert.Null(request.StopPlaceOrStation.Dhids);
        Assert.Null(request.StopPlaceOrStation.Platforms);
        Assert.Null(request.StopPlaceOrStation.BoardConfigurations);
        Assert.Null(request.TransportTypes);
    }

    [Fact]
    public void ToDepartureContract_WithUtcTimes_FormatsWithoutOffset() {
        var query = new BoardQuery {
            Start = new DateTime(2026, 11, 23, 9, 30, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 11, 23, 11, 30, 0, DateTimeKind.Utc)
        };

        var request = QueryUtil.ToDepartureContract(query);
        var timeWindow = request.TimeWindow!;

        Assert.Equal("2026-11-23T09:30:00Z", timeWindow.TimeStart);
        Assert.Equal("2026-11-23T11:30:00Z", timeWindow.TimeEnd);
    }

    [Fact]
    public void ToDepartureContract_WithEntryCount_MapsTimeWindowWithoutDates() {
        var query = new BoardQuery {
            Start = new DateTime(2026, 11, 23, 10, 0, 0, DateTimeKind.Unspecified),
            EntryCount = 10
        };

        var request = QueryUtil.ToDepartureContract(query);
        var timeWindow = request.TimeWindow!;

        Assert.Equal("2026-11-23T10:00:00Z", timeWindow.TimeStart);
        Assert.Null(timeWindow.TimeEnd);
        Assert.Equal(10, timeWindow.EntryCount);
    }

    [Fact]
    public void ToArrivalContract_WithoutAnyTimeWindow_MapsToNull() {
        var request = QueryUtil.ToArrivalContract(new BoardQuery());

        Assert.Null(request.TimeWindow);
    }

    [Fact]
    public void ToArrivalContract_AndToDepartureContract_ProduceEquivalentRequests() {
        var query = new BoardQuery()
                    .ForEvaNumbers("8000105")
                    .StartingAt(new DateTime(2026, 11, 23, 10, 0, 0, DateTimeKind.Unspecified))
                    .WithPlatformAccessibility();

        var arrivalRequest = QueryUtil.ToArrivalContract(query);
        var departureRequest = QueryUtil.ToDepartureContract(query);

        Assert.Equivalent(arrivalRequest.StopPlaceOrStation, departureRequest.StopPlaceOrStation);
        Assert.Equivalent(arrivalRequest.TimeWindow, departureRequest.TimeWindow);
        Assert.Equivalent(arrivalRequest.Languages, departureRequest.Languages);
        Assert.Equal(arrivalRequest.MaxEntriesPerTransportType, departureRequest.MaxEntriesPerTransportType);
        Assert.Equal(arrivalRequest.MaxViaStops, departureRequest.MaxViaStops);
        Assert.Equal(arrivalRequest.SortBy, departureRequest.SortBy);
        Assert.Equal(arrivalRequest.IncludePlatformAccessibility, departureRequest.IncludePlatformAccessibility);
    }
}
