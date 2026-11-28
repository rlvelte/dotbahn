using DotBahn.Common.Models;
using DotBahn.Timetables.Models;
using DotBahn.Timetables.Models.Enumerations;
using Shouldly;

namespace DotBahn.Tests.Timetables.Models;

public class TimetableModelTests {
    [Theory]
    [InlineData(true, false, true, false, false)]
    [InlineData(false, true, false, true, false)]
    [InlineData(true, true, false, false, true)]
    [InlineData(false, false, false, false, false)]
    public void StopFlags_Should_ReflectEventPresence(bool hasArrival, bool hasDeparture, bool arrivalOnly, bool departureOnly, bool through) {
        var sut = CreateStop(hasArrival, hasDeparture);

        sut.IsArrivalOnly.ShouldBe(arrivalOnly);
        sut.IsDepartureOnly.ShouldBe(departureOnly);
        sut.IsThrough.ShouldBe(through);
    }

    [Theory]
    [InlineData(null, "ICE123")]
    [InlineData("", "ICE123")]
    [InlineData("   ", "ICE123")]
    [InlineData("RE14", "RE14")]
    public void DisplayName_Should_PreferLine_When_ItIsNotWhitespace(string? line, string expected) {
        var sut = new TrainLabel {
            Category = "ICE",
            Number = "123",
            Owner = "DB",
            Line = line
        };

        sut.DisplayName.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, null, true)]
    [InlineData(null, 1, true)]
    [InlineData(null, -1, false)]
    [InlineData(true, 1, false)]
    public void IsActive_Should_DependOnDeletionAndValidity(bool? deleted, int? validToDaysFromNow, bool expected) {
        var sut = new TimetableMessage {
            Id = "m1",
            Type = MessageType.Him,
            Timestamp = DateTime.UnixEpoch,
            IsDeleted = deleted ?? false,
            ValidTo = validToDaysFromNow.HasValue ? DateTime.Now.AddDays(validToDaysFromNow.Value) : null
        };

        sut.IsActive.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("internal", null, "internal")]
    [InlineData("internal", "external", "external")]
    public void Text_Should_PreferExternalText_When_Present(string? internalText, string? externalText, string? expected) {
        var sut = new TimetableMessage {
            Id = "m1",
            Type = MessageType.Him,
            Timestamp = DateTime.UnixEpoch,
            InternalText = internalText,
            ExternalText = externalText
        };

        sut.Text.ShouldBe(expected);
    }

    private static TimetableStop CreateStop(bool hasArrival, bool hasDeparture) {
        var train = new TrainLabel { Category = "ICE", Number = "1", Owner = "DB" };
        var arrival = hasArrival ? new TrainEvent {
            Time = new ChangedValue<DateTime> { Original = DateTime.UnixEpoch },
            Platform = new ChangedRef<string> { Original = "1" },
            Status = new ChangedValue<EventStatus> { Original = EventStatus.Planned },
            DistantEndpoint = new ChangedRef<string> { Original = string.Empty }
        } : null;
        var departure = hasDeparture ? new TrainEvent {
            Time = new ChangedValue<DateTime> { Original = DateTime.UnixEpoch },
            Platform = new ChangedRef<string> { Original = "1" },
            Status = new ChangedValue<EventStatus> { Original = EventStatus.Planned },
            DistantEndpoint = new ChangedRef<string> { Original = string.Empty }
        } : null;

        return new TimetableStop { Id = "1", Train = train, Arrival = arrival, Departure = departure };
    }
}
