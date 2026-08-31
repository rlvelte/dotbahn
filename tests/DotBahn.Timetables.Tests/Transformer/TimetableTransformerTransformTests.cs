using DotBahn.Timetables.Internal.Contracts;
using DotBahn.Timetables.Internal.Transformers;
using DotBahn.Timetables.Models.Enumerations;

namespace DotBahn.Timetables.Tests.Transformer;

public class TimetableTransformerTransformTests {
    private readonly TimetableTransformer _transformer = new();

    [Fact]
    public void Transform_WithBasicContract_ShouldSetStation() {
        var contract = new TimetableResponseContract {
            Station = "Hamburg Hbf",
            Stops = []
        };

        var result = _transformer.Transform(contract);

        Assert.Equal("Hamburg Hbf", result.Station);
        Assert.Empty(result.Stops);
        Assert.Empty(result.Messages);
    }

    [Fact]
    public void Transform_WithStop_ShouldTransformStopData() {
        var contract = new TimetableResponseContract {
            Station = "Berlin Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-1",
                    TripInfo = new TripInfoContract {
                        Category = "ICE",
                        Number = "123",
                        Owner = "80"
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var stop = Assert.Single(result.Stops);
        Assert.Equal("stop-1", stop.Id);
        Assert.Equal("ICE", stop.Train.Category);
        Assert.Equal("123", stop.Train.Number);
        Assert.Equal("80", stop.Train.Owner);
    }

    [Fact]
    public void Transform_WithDeparture_ShouldTransformEvent() {
        var contract = new TimetableResponseContract {
            Station = "München Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-1",
                    Departure = new EventContract {
                        PlannedTime = "2501191430",
                        PlannedPlatform = "12",
                        PlannedPath = "Augsburg|Nürnberg|Frankfurt"
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var stop = Assert.Single(result.Stops);
        Assert.NotNull(stop.Departure);
        Assert.Null(stop.Arrival);
        Assert.Equal(new DateTime(2025, 1, 19, 14, 30, 0), stop.Departure.Time.Original);
        Assert.Equal("12", stop.Departure.Platform.Original);
        Assert.Equal(["Augsburg", "Nürnberg", "Frankfurt"], stop.Departure.Path.Original);
    }

    [Fact]
    public void Transform_WithArrival_ShouldTransformEvent() {
        var contract = new TimetableResponseContract {
            Station = "Köln Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-2",
                    Arrival = new EventContract {
                        PlannedTime = "2501191200",
                        PlannedPlatform = "5"
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var stop = Assert.Single(result.Stops);
        Assert.NotNull(stop.Arrival);
        Assert.Null(stop.Departure);
        Assert.Equal(new DateTime(2025, 1, 19, 12, 0, 0), stop.Arrival.Time.Original);
        Assert.Equal("5", stop.Arrival.Platform.Original);
    }

    [Fact]
    public void Transform_WithChangedValues_ShouldSetUpdatedProperties() {
        var contract = new TimetableResponseContract {
            Station = "Stuttgart Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-3",
                    Departure = new EventContract {
                        PlannedTime = "2501191000",
                        PlannedPlatform = "8",
                        ChangedTime = "2501191015",
                        ChangedPlatform = "9"
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var departure = result.Stops.First().Departure!;
        Assert.Equal(new DateTime(2025, 1, 19, 10, 0, 0), departure.Time.Original);
        Assert.Equal(new DateTime(2025, 1, 19, 10, 15, 0), departure.Time.Updated);
        Assert.True(departure.Time.HasUpdate);
        Assert.Equal("8", departure.Platform.Original);
        Assert.Equal("9", departure.Platform.Updated);
        Assert.True(departure.Platform.HasUpdate);
    }

    [Fact]
    public void Transform_WithoutChangedValues_ShouldNotHaveUpdates() {
        var contract = new TimetableResponseContract {
            Station = "Düsseldorf Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-4",
                    Departure = new EventContract {
                        PlannedTime = "2501191800",
                        PlannedPlatform = "3"
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var departure = result.Stops.First().Departure!;
        Assert.False(departure.Time.HasUpdate);
        Assert.False(departure.Platform.HasUpdate);
        Assert.Equal(departure.Time.Original, departure.Time.Actual);
        Assert.Equal(departure.Platform.Original, departure.Platform.Actual);
    }

    [Fact]
    public void Transform_WithChangedStatus_ShouldSetStatusUpdate() {
        var contract = new TimetableResponseContract {
            Station = "Leipzig Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-5",
                    Departure = new EventContract {
                        PlannedTime = "2501191600",
                        PlannedStatus = "p",
                        ChangedStatus = "c"
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var departure = result.Stops.First().Departure!;
        Assert.Equal(EventStatus.Planned, departure.Status.Original);
        Assert.Equal(EventStatus.Cancelled, departure.Status.Updated);
        Assert.True(departure.Status.HasUpdate);
    }

    [Fact]
    public void Transform_WithDistantEndpoint_ShouldSetDistantEndpoint() {
        var contract = new TimetableResponseContract {
            Station = "Basel SBB",
            Stops = [
                new StopDataContract {
                    Id = "stop-6",
                    Departure = new EventContract {
                        PlannedTime = "2501191400",
                        PlannedDistantEndpoint = "Milano Centrale",
                        ChangedDistantEndpoint = "Roma Termini"
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var departure = result.Stops.First().Departure!;
        Assert.NotNull(departure.DistantEndpoint);
        Assert.Equal("Milano Centrale", departure.DistantEndpoint.Original);
        Assert.Equal("Roma Termini", departure.DistantEndpoint.Updated);
    }

    [Fact]
    public void Transform_WithMultipleStops_ShouldTransformAll() {
        var contract = new TimetableResponseContract {
            Station = "Frankfurt Hbf",
            Stops = [
                new StopDataContract { Id = "stop-a" },
                new StopDataContract { Id = "stop-b" },
                new StopDataContract { Id = "stop-c" }
            ]
        };

        var result = _transformer.Transform(contract);

        Assert.Equal(3, result.Stops.Count());
        Assert.Contains(result.Stops, s => s.Id == "stop-a");
        Assert.Contains(result.Stops, s => s.Id == "stop-b");
        Assert.Contains(result.Stops, s => s.Id == "stop-c");
    }

    [Fact]
    public void Transform_WithNullContract_ThrowsArgumentNullException() {
        TimetableResponseContract contract = null!;

        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(contract));
        Assert.Equal("contracts", exception.ParamName);
    }

    [Fact]
    public void Transform_WithNullTripInfo_ShouldUseDefaultValues() {
        var contract = new TimetableResponseContract {
            Station = "Hannover Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-1",
                    TripInfo = null
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var stop = Assert.Single(result.Stops);
        Assert.Equal(string.Empty, stop.Train.Category);
        Assert.Equal(string.Empty, stop.Train.Number);
        Assert.Equal(string.Empty, stop.Train.Owner);
        Assert.Null(stop.Train.Type);
    }

    [Fact]
    public void Transform_WithNullPlannedPath_ShouldDefaultToEmptyList() {
        var contract = new TimetableResponseContract {
            Station = "Dortmund Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-1",
                    Departure = new EventContract {
                        PlannedTime = "2501191000",
                        PlannedPath = null
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var departure = result.Stops.First().Departure!;
        Assert.Empty(departure.Path.Original);
        Assert.Null(departure.Path.Updated);
        Assert.False(departure.Path.HasUpdate);
    }

    [Fact]
    public void Transform_WithNullWings_ShouldDefaultToEmptyList() {
        var contract = new TimetableResponseContract {
            Station = "Bremen Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-1",
                    Departure = new EventContract {
                        PlannedTime = "2501191000",
                        Wings = null
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var departure = result.Stops.First().Departure!;
        Assert.Empty(departure.Wings);
    }

    public static TheoryData<string, string, DateTime?> ParseBahnTimeEdgeCases => new()
    {
        { "BL—valid", "2501191430", new DateTime(2025, 1, 19, 14, 30, 0) },
        { "B1—leap_year", "2402291200", new DateTime(2024, 2, 29, 12, 0, 0) },
        { "B2—non_leap_feb29", "2302291200", null },
        { "B3—midnight", "2501190000", new DateTime(2025, 1, 19, 0, 0, 0) },
        { "B4—end_of_day", "2501192359", new DateTime(2025, 1, 19, 23, 59, 0) },
        { "B5—invalid_month", "2501320000", null },
        { "B6—invalid_day", "2502301200", null },
        { "B7—invalid_hour", "2501192400", null },
        { "B8—invalid_minute", "2501190060", null },
        { "B9—year_rollover", "9912311200", new DateTime(1999, 12, 31, 12, 0, 0) },
        { "B10—century_leap", "0002291200", new DateTime(2000, 2, 29, 12, 0, 0) },
        { "B11—invalid_century_day", "0102291200", null },
    };

    [Theory]
    [MemberData(nameof(ParseBahnTimeEdgeCases))]
    public void ParseBahnTimeEdge(string _, string plannedTime, DateTime? expected) {
        var contract = new TimetableResponseContract {
            Station = "Test",
            Stops = [new StopDataContract { Id = "s1", Departure = new EventContract { PlannedTime = plannedTime } }],
        };
        var result = _transformer.Transform(contract);
        var departure = result.Stops.First().Departure!;

        Assert.Equal(expected ?? default, departure.Time.Original);
    }

    [Fact]
    public void TransformWithUnrecognizedTripTypeDefaultsToPassenger() {
        var contract = new TimetableResponseContract {
            Station = "Test",
            Stops = [new StopDataContract {
                Id = "s1",
                TripInfo = new TripInfoContract { TripType = "UNKNOWN_TYPE" }
            }]
        };

        var result = _transformer.Transform(contract);

        Assert.Equal(TripType.Passenger, result.Stops.First().Train.Type);
    }

    [Fact]
    public void Transform_WithChangedStatusNull_ShouldNotSetUpdate() {
        var contract = new TimetableResponseContract {
            Station = "Nürnberg Hbf",
            Stops = [
                new StopDataContract {
                    Id = "stop-1",
                    Departure = new EventContract {
                        PlannedTime = "2501191000",
                        PlannedStatus = "p",
                        ChangedStatus = null
                    }
                }
            ]
        };

        var result = _transformer.Transform(contract);

        var departure = result.Stops.First().Departure!;
        Assert.Equal(EventStatus.Planned, departure.Status.Original);
        Assert.Null(departure.Status.Updated);
        Assert.False(departure.Status.HasUpdate);
    }
}
