using DotBahn.Data.Shared.Models;
using DotBahn.Data.Timetables.Models;

namespace DotBahn.Tests.Timetables.Models;

public class TimetableStopTests {
    private static TrainLabel EmptyTrain => new() { Category = "", Number = "", Owner = "" };
    private static TrainEvent AnyEvent => new() {
        Time = new ChangedValue<DateTime> { Original = DateTime.MinValue },
        Platform = new ChangedRef<string> { Original = "" },
        Status = new ChangedValue<DotBahn.Data.Timetables.Enumerations.EventStatus> { Original = DotBahn.Data.Timetables.Enumerations.EventStatus.Unknown },
        DistantEndpoint = new ChangedRef<string> { Original = "" },
        Path = new ChangedRef<IEnumerable<string>> { Original = [] }
    };

    [Fact]
    public void IsArrivalOnly_ArrivalSetDepartureNull_ReturnsTrue() {
        // Arrange
        var stop = new TimetableStop { Id = "x", Train = EmptyTrain, Arrival = AnyEvent };

        // Assert
        Assert.True(stop.IsArrivalOnly);
        Assert.False(stop.IsDepartureOnly);
        Assert.False(stop.IsThrough);
    }

    [Fact]
    public void IsDepartureOnly_DepartureSetArrivalNull_ReturnsTrue() {
        // Arrange
        var stop = new TimetableStop { Id = "x", Train = EmptyTrain, Departure = AnyEvent };

        // Assert
        Assert.True(stop.IsDepartureOnly);
        Assert.False(stop.IsArrivalOnly);
        Assert.False(stop.IsThrough);
    }

    [Fact]
    public void IsThrough_BothSet_ReturnsTrue() {
        // Arrange
        var stop = new TimetableStop { Id = "x", Train = EmptyTrain, Arrival = AnyEvent, Departure = AnyEvent };

        // Assert
        Assert.True(stop.IsThrough);
        Assert.False(stop.IsArrivalOnly);
        Assert.False(stop.IsDepartureOnly);
    }

    [Fact]
    public void AllFalse_WhenBothNull() {
        // Arrange
        var stop = new TimetableStop { Id = "x", Train = EmptyTrain };

        // Assert
        Assert.False(stop.IsArrivalOnly);
        Assert.False(stop.IsDepartureOnly);
        Assert.False(stop.IsThrough);
    }
}
