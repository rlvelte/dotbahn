using DotBahn.Clients.Timetables.Utilities;
using FluentAssertions;

namespace DotBahn.Tests.Timetables.Transformer;

public class TransformerUtilsTests {
    [Fact]
    public void ParseApiTime_ShouldParseCorrectly() {
        // Arrange
        var timeStr = "2601072030";

        // Act
        var result = TransformerUtils.ParseApiTime(timeStr);

        // Assert
        result.Should().Be(new DateTime(2026, 1, 7, 20, 30, 0));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789")]
    [InlineData("12345678901")]
    [InlineData("YYMMDDhhmm")]
    public void ParseApiTime_ShouldThrow_WhenFormatIsInvalid(string invalidTimeStr) {
        // Act
        var act = () => TransformerUtils.ParseApiTime(invalidTimeStr);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid time format*");
    }

    [Fact]
    public void ParsePath_ShouldParseCorrectly() {
        // Arrange
        var pathStr = "Station A|Station B|Station C";

        // Act
        var result = TransformerUtils.ParsePath(pathStr);

        // Assert
        result.Should().HaveCount(3).And.ContainInOrder("Station A", "Station B", "Station C");
    }

    [Fact]
    public void ParsePath_ShouldReturnEmpty_WhenInputIsNull() {
        // Act
        var result = TransformerUtils.ParsePath(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParsePath_ShouldReturnEmpty_WhenInputIsEmpty() {
        // Act
        var result = TransformerUtils.ParsePath("");

        // Assert
        result.Should().BeEmpty();
    }
}
