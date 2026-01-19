using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Transformer;
using DotBahn.Data.Timetables.Enumerations;

namespace DotBahn.Tests.Timetables.Transformer;

public class TimetableTransformerTransformTests {
    private readonly TimetableTransformer _transformer = new();

    [Fact]
    public void Transform_WithBasicContract_ShouldSetStation() {
        // Arrange
        var contract = new TimetableResponseContract {
            Station = "Hamburg Hbf",
            Stops = []
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        Assert.Equal("Hamburg Hbf", result.Station);
        Assert.Empty(result.Stops);
        Assert.Empty(result.Messages);
    }

    [Fact]
    public void Transform_WithStop_ShouldTransformStopData() {
        // Arrange
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

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        var stop = Assert.Single(result.Stops);
        Assert.Equal("stop-1", stop.Id);
        Assert.Equal("ICE", stop.Train.Category);
        Assert.Equal("123", stop.Train.Number);
        Assert.Equal("80", stop.Train.Owner);
    }

    [Fact]
    public void Transform_WithDeparture_ShouldTransformEvent() {
        // Arrange
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

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        var stop = Assert.Single(result.Stops);
        Assert.NotNull(stop.Departure);
        Assert.Null(stop.Arrival);
        Assert.Equal(new DateTime(2025, 1, 19, 14, 30, 0), stop.Departure.Time.Original);
        Assert.Equal("12", stop.Departure.Platform.Original);
        Assert.Equal(["Augsburg", "Nürnberg", "Frankfurt"], stop.Departure.Path.Original);
    }

    [Fact]
    public void Transform_WithArrival_ShouldTransformEvent() {
        // Arrange
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

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        var stop = Assert.Single(result.Stops);
        Assert.NotNull(stop.Arrival);
        Assert.Null(stop.Departure);
        Assert.Equal(new DateTime(2025, 1, 19, 12, 0, 0), stop.Arrival.Time.Original);
        Assert.Equal("5", stop.Arrival.Platform.Original);
    }

    [Fact]
    public void Transform_WithChangedValues_ShouldSetUpdatedProperties() {
        // Arrange
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

        // Act
        var result = _transformer.Transform(contract);

        // Assert
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
        // Arrange
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

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        var departure = result.Stops.First().Departure!;
        Assert.False(departure.Time.HasUpdate);
        Assert.False(departure.Platform.HasUpdate);
        Assert.Equal(departure.Time.Original, departure.Time.Actual);
        Assert.Equal(departure.Platform.Original, departure.Platform.Actual);
    }

    [Fact]
    public void Transform_WithChangedStatus_ShouldSetStatusUpdate() {
        // Arrange
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

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        var departure = result.Stops.First().Departure!;
        Assert.Equal(EventStatus.Planned, departure.Status.Original);
        Assert.Equal(EventStatus.Cancelled, departure.Status.Updated);
        Assert.True(departure.Status.HasUpdate);
    }

    [Fact]
    public void Transform_WithDistantEndpoint_ShouldSetDistantEndpoint() {
        // Arrange
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

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        var departure = result.Stops.First().Departure!;
        Assert.NotNull(departure.DistantEndpoint);
        Assert.Equal("Milano Centrale", departure.DistantEndpoint.Original);
        Assert.Equal("Roma Termini", departure.DistantEndpoint.Updated);
    }

    [Fact]
    public void Transform_WithMultipleStops_ShouldTransformAll() {
        // Arrange
        var contract = new TimetableResponseContract {
            Station = "Frankfurt Hbf",
            Stops = [
                new StopDataContract { Id = "stop-a" },
                new StopDataContract { Id = "stop-b" },
                new StopDataContract { Id = "stop-c" }
            ]
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        Assert.Equal(3, result.Stops.Count());
        Assert.Contains(result.Stops, s => s.Id == "stop-a");
        Assert.Contains(result.Stops, s => s.Id == "stop-b");
        Assert.Contains(result.Stops, s => s.Id == "stop-c");
    }
}
