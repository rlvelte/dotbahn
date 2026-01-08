using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Enumerations;
using DotBahn.Clients.Timetables.Transformers;
using FluentAssertions;

namespace DotBahn.Tests.Timetables.Transformer;

public class MessageTransformerTests {
    private readonly MessageTransformer _transformer = new();

    [Fact]
    public void Transform_ShouldMapBasicProperties() {
        // Arrange
        var contract = new MessageContract {
            Id = "m1",
            Type = "q",
            Code = "80",
            Text = "Test Message",
            Priority = "1",
            IsInternal = "1",
            IsDeleted = "0"
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        result.Id.Should().Be("m1");
        result.Type.Should().Be(MessageType.Quality);
        result.Code.Should().Be("80");
        result.Text.Should().Be("Test Message");
        result.Priority.Should().Be(1);
        result.IsInternal.Should().BeTrue();
        result.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Transform_ShouldMapTimestamps() {
        // Arrange
        var contract = new MessageContract {
            Id = "m2",
            ValidFrom = "2601072000",
            ValidTo = "2601072100",
            Timestamp = "2601072030"
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        result.ValidFrom.Should().Be(new DateTime(2026, 1, 7, 20, 0, 0));
        result.ValidTo.Should().Be(new DateTime(2026, 1, 7, 21, 0, 0));
        result.Timestamp.Should().Be(new DateTime(2026, 1, 7, 20, 30, 0));
    }

    [Fact]
    public void Transform_ShouldHandleEmptyValues() {
        // Arrange
        var contract = new MessageContract {
            Id = null,
            Type = null,
            Priority = null
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        result.Id.Should().BeEmpty();
        result.Type.Should().Be(MessageType.Quality); // Default value from EnumExtensions.FromAssociatedValue
        result.Priority.Should().BeNull();
    }
}
