using DotBahn.Clients.Timetables.Transformer;
using DotBahn.Data.Shared.Models;
using DotBahn.Data.Timetables.Enumerations;
using DotBahn.Data.Timetables.Models;

namespace DotBahn.Tests.Timetables.Transformer;

public class TimetableTransformerMergeTests {
    private readonly TimetableTransformer _transformer = new();

    [Fact]
    public void Merge_WithNoChanges_ShouldReturnCurrent() {
        // Arrange
        var current = CreateTimetable("Hamburg Hbf", [CreateStop("stop-1")]);
        var changes = CreateTimetable("Hamburg Hbf", []);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        Assert.Equal("Hamburg Hbf", result.Station);
        Assert.Single(result.Stops);
    }

    [Fact]
    public void Merge_WithNewStop_ShouldAddStop() {
        // Arrange
        var current = CreateTimetable("Berlin Hbf", [CreateStop("stop-1")]);
        var changes = CreateTimetable("Berlin Hbf", [CreateStop("stop-2")]);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        Assert.Equal(2, result.Stops.Count());
        Assert.Contains(result.Stops, s => s.Id == "stop-1");
        Assert.Contains(result.Stops, s => s.Id == "stop-2");
    }

    [Fact]
    public void Merge_WithExistingStop_ShouldUpdateStop() {
        // Arrange
        var current = CreateTimetable("München Hbf", [
            CreateStopWithDeparture("stop-1", "2501191000", "5")
        ]);
        var changes = CreateTimetable("München Hbf", [
            CreateStopWithDepartureUpdate("stop-1", "2501191015", "6")
        ]);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        var stop = Assert.Single(result.Stops);
        Assert.Equal("stop-1", stop.Id);
        Assert.Equal(new DateTime(2025, 1, 19, 10, 0, 0), stop.Departure!.Time.Original);
        Assert.Equal(new DateTime(2025, 1, 19, 10, 15, 0), stop.Departure.Time.Updated);
        Assert.Equal("5", stop.Departure.Platform.Original);
        Assert.Equal("6", stop.Departure.Platform.Updated);
    }

    [Fact]
    public void Merge_PreservesOriginalValues() {
        // Arrange
        var current = CreateTimetable("Köln Hbf", [
            CreateStopWithDeparture("stop-1", "2501191200", "8")
        ]);
        var changes = CreateTimetable("Köln Hbf", [
            CreateStopWithDepartureUpdate("stop-1", "2501191230", null)
        ]);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        var departure = result.Stops.First().Departure!;
        Assert.Equal(new DateTime(2025, 1, 19, 12, 0, 0), departure.Time.Original);
        Assert.Equal("8", departure.Platform.Original);
    }

    [Fact]
    public void Merge_WithNoUpdate_ShouldKeepCurrentUpdate() {
        // Arrange
        var current = CreateTimetable("Stuttgart Hbf", [
            CreateStopWithExistingUpdate("stop-1", "2501191400", "2501191415")
        ]);
        var changes = CreateTimetable("Stuttgart Hbf", [
            CreateStopWithDeparture("stop-1", "2501191400", "10")
        ]);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        var departure = result.Stops.First().Departure!;
        Assert.Equal(new DateTime(2025, 1, 19, 14, 15, 0), departure.Time.Updated);
    }

    [Fact]
    public void Merge_WithMessages_ShouldCombineWithoutDuplicates() {
        // Arrange
        var current = CreateTimetable("Frankfurt Hbf", [], [
            CreateMessage("msg-1"),
            CreateMessage("msg-2")
        ]);
        var changes = CreateTimetable("Frankfurt Hbf", [], [
            CreateMessage("msg-2"),
            CreateMessage("msg-3")
        ]);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        Assert.Equal(3, result.Messages.Count());
        Assert.Contains(result.Messages, m => m.Id == "msg-1");
        Assert.Contains(result.Messages, m => m.Id == "msg-2");
        Assert.Contains(result.Messages, m => m.Id == "msg-3");
    }

    [Fact]
    public void Merge_PreservesStation() {
        // Arrange
        var current = CreateTimetable("Leipzig Hbf", []);
        var changes = CreateTimetable("Other Station", []);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        Assert.Equal("Leipzig Hbf", result.Station);
    }

    [Fact]
    public void Merge_PreservesTrainLabel() {
        // Arrange
        var current = CreateTimetable("Dresden Hbf", [
            new TimetableStop {
                Id = "stop-1",
                Train = new TrainLabel { Category = "ICE", Number = "123", Owner = "80" }
            }
        ]);
        var changes = CreateTimetable("Dresden Hbf", [
            new TimetableStop {
                Id = "stop-1",
                Train = new TrainLabel { Category = "RE", Number = "456", Owner = "99" }
            }
        ]);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        var stop = Assert.Single(result.Stops);
        Assert.Equal("ICE", stop.Train.Category);
        Assert.Equal("123", stop.Train.Number);
        Assert.Equal("80", stop.Train.Owner);
    }

    [Fact]
    public void Merge_WithNullCurrentEvent_ShouldTakeChange() {
        // Arrange
        var current = CreateTimetable("Hannover Hbf", [CreateStop("stop-1")]);
        var changes = CreateTimetable("Hannover Hbf", [
            CreateStopWithDeparture("stop-1", "2501191600", "12")
        ]);

        // Act
        var result = _transformer.Merge(current, changes);

        // Assert
        var stop = Assert.Single(result.Stops);
        Assert.NotNull(stop.Departure);
        Assert.Equal(new DateTime(2025, 1, 19, 16, 0, 0), stop.Departure.Time.Original);
    }

    #region Helper Methods

    private static Timetable CreateTimetable(string station, IEnumerable<TimetableStop> stops, IEnumerable<TimetableMessage>? messages = null) => new() {
        Station = station,
        Stops = stops,
        Messages = messages ?? []
    };

    private static TimetableStop CreateStop(string id) => new() {
        Id = id,
        Train = new TrainLabel { Category = "ICE", Number = "1", Owner = "80" }
    };

    private static TimetableStop CreateStopWithDeparture(string id, string time, string platform) => new() {
        Id = id,
        Train = new TrainLabel { Category = "ICE", Number = "1", Owner = "80" },
        Departure = new TrainEvent {
            Time = new ChangedValue<DateTime> { Original = ParseTime(time) },
            Platform = new ChangedRef<string> { Original = platform },
            Status = new ChangedValue<EventStatus> { Original = EventStatus.Planned },
            DistantEndpoint = new ChangedRef<string> { Original = string.Empty },
            Path = new ChangedRef<IEnumerable<string>> { Original = [] }
        }
    };

    private static TimetableStop CreateStopWithDepartureUpdate(string id, string? updatedTime, string? updatedPlatform) => new() {
        Id = id,
        Train = new TrainLabel { Category = "ICE", Number = "1", Owner = "80" },
        Departure = new TrainEvent {
            Time = new ChangedValue<DateTime> {
                Original = default,
                Updated = updatedTime != null ? ParseTime(updatedTime) : null
            },
            Platform = new ChangedRef<string> {
                Original = string.Empty,
                Updated = updatedPlatform
            },
            Status = new ChangedValue<EventStatus> { Original = EventStatus.Planned },
            DistantEndpoint = new ChangedRef<string> { Original = string.Empty },
            Path = new ChangedRef<IEnumerable<string>> { Original = [] }
        }
    };

    private static TimetableStop CreateStopWithExistingUpdate(string id, string originalTime, string updatedTime) => new() {
        Id = id,
        Train = new TrainLabel { Category = "ICE", Number = "1", Owner = "80" },
        Departure = new TrainEvent {
            Time = new ChangedValue<DateTime> {
                Original = ParseTime(originalTime),
                Updated = ParseTime(updatedTime)
            },
            Platform = new ChangedRef<string> { Original = "10" },
            Status = new ChangedValue<EventStatus> { Original = EventStatus.Planned },
            DistantEndpoint = new ChangedRef<string> { Original = string.Empty },
            Path = new ChangedRef<IEnumerable<string>> { Original = [] }
        }
    };

    private static TimetableMessage CreateMessage(string id) => new() {
        Id = id,
        Type = MessageType.Him,
        Timestamp = DateTime.Now
    };

    private static DateTime ParseTime(string time) =>
        DateTime.ParseExact(time, "yyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);

    #endregion
}
