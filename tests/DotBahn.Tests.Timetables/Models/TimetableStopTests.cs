using DotBahn.Data.Shared.Models;
using DotBahn.Data.Timetables.Enumerations;
using DotBahn.Data.Timetables.Models;

namespace DotBahn.Tests.Timetables.Models;

public class TimetableStopTests {
    private static TrainLabel EmptyTrain => new() { Category = "", Number = "", Owner = "" };
    private static TrainEvent AnyEvent => new() {
        Time = new ChangedValue<DateTime> { Original = DateTime.MinValue },
        Platform = new ChangedRef<string> { Original = "" },
        Status = new ChangedValue<EventStatus> { Original = EventStatus.Unknown },
        DistantEndpoint = new ChangedRef<string> { Original = "" },
        Path = new ChangedRef<IEnumerable<string>> { Original = [] }
    };

    [Theory]
    [InlineData(true, false, true, false, false)]
    [InlineData(false, true, false, true, false)]
    [InlineData(true, true, false, false, true)]
    [InlineData(false, false, false, false, false)]
    public void StopType_ReturnsCorrectFlags(bool hasArrival, bool hasDeparture,
        bool expectedArrivalOnly, bool expectedDepartureOnly, bool expectedThrough) {
        var stop = new TimetableStop {
            Id = "x",
            Train = EmptyTrain,
            Arrival = hasArrival ? AnyEvent : null,
            Departure = hasDeparture ? AnyEvent : null
        };

        Assert.Equal(expectedArrivalOnly, stop.IsArrivalOnly);
        Assert.Equal(expectedDepartureOnly, stop.IsDepartureOnly);
        Assert.Equal(expectedThrough, stop.IsThrough);
    }
}
