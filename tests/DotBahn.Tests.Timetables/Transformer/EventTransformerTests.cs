using DotBahn.Timetables.Contracts;
using DotBahn.Timetables.Enumerations;
using DotBahn.Timetables.Transformers;
using FluentAssertions;

namespace DotBahn.Tests.Timetables.Transformer;

public class EventTransformerTests {
    private readonly EventTransformer _transformer = new();

    [Fact]
    public void Transform_ShouldReturnNull_WhenPlannedTimeIsEmpty() {
        // Arrange
        var contract = new EventContract { PlannedTime = "" };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Transform_ShouldMapBasicProperties() {
        // Arrange
        var contract = new EventContract {
            PlannedTime = "2601072030", // 2026-01-07 20:30
            Line = "S1",
            IsHidden = "1",
            PlannedStatus = "p",
            PlannedPlatform = "1",
            PlannedPath = "Station A|Station B",
            PlannedDistantEndpoint = "Endpoint A"
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        result.Should().NotBeNull();
        result!.Line.Should().Be("S1");
        result.IsHidden.Should().BeTrue();
        result.Status.Should().Be(EventStatus.Planned);
        result.Time.Planned.Should().Be(new DateTime(2026, 1, 7, 20, 30, 0));
        result.Platform.Planned.Should().Be("1");
        result.Path.Planned.Should().ContainInOrder("Station A", "Station B");
        result.DistantEndpoint.Planned.Should().Be("Endpoint A");
    }

    [Fact]
    public void Transform_ShouldMapChangedProperties() {
        // Arrange
        var contract = new EventContract {
            PlannedTime = "2601072030",
            ChangedTime = "2601072035",
            ChangedPlatform = "2",
            ChangedPath = "Station A|Station C",
            ChangedDistantEndpoint = "Endpoint B"
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        result.Should().NotBeNull();
        result!.Time.Changed.Should().Be(new DateTime(2026, 1, 7, 20, 35, 0));
        result.Platform.Changed.Should().Be("2");
        result.Path.Changed.Should().ContainInOrder("Station A", "Station C");
        result.DistantEndpoint.Changed.Should().Be("Endpoint B");
    }
}
