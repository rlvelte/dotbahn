using DotBahn.Common.Models;
using Shouldly;

namespace DotBahn.Common.Tests.Models;

public class ChangedValueTests {
    [Fact]
    public void Actual_Should_ReturnOriginal_When_UpdatedIsNull() {
        var sut = new ChangedValue<int> {
            Original = 42
        };

        sut.Actual.ShouldBe(42);
    }

    [Fact]
    public void Actual_Should_ReturnUpdated_When_UpdatedIsSet() {
        var sut = new ChangedValue<int> {
            Original = 42,
            Updated = 99
        };

        sut.Actual.ShouldBe(99);
    }

    [Fact]
    public void HasUpdate_Should_BeFalse_When_UpdatedIsNull() {
        var sut = new ChangedValue<int> {
            Original = 42
        };

        sut.HasUpdate.ShouldBeFalse();
    }

    [Fact]
    public void HasUpdate_Should_BeTrue_When_UpdatedIsSet() {
        var sut = new ChangedValue<int> {
            Original = 42,
            Updated = 99
        };

        sut.HasUpdate.ShouldBeTrue();
    }

    [Fact]
    public void HasUpdate_Should_BeTrue_When_UpdatedIsZero() {
        var sut = new ChangedValue<int> {
            Original = 42,
            Updated = 0
        };

        sut.HasUpdate.ShouldBeTrue();
        sut.Actual.ShouldBe(0);
    }

    [Fact]
    public void Actual_Should_ReturnOriginal_When_UpdatedIsNullForNullableValue() {
        var sut = new ChangedValue<DateTime> {
            Original = new DateTime(2024, 1, 1)
        };

        sut.Actual.ShouldBe(new DateTime(2024, 1, 1));
        sut.HasUpdate.ShouldBeFalse();
    }

    [Fact]
    public void Actual_Should_ReturnUpdated_When_UpdatedIsSetForNullableValue() {
        var sut = new ChangedValue<DateTime> {
            Original = new DateTime(2024, 1, 1),
            Updated = new DateTime(2024, 12, 31)
        };

        sut.Actual.ShouldBe(new DateTime(2024, 12, 31));
        sut.HasUpdate.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(-10, -5, -5)]
    [InlineData(int.MaxValue, int.MinValue, int.MinValue)]
    public void Actual_Should_ReturnUpdated_ForBoundaryValues(int original, int updated, int expected) {
        var sut = new ChangedValue<int> {
            Original = original,
            Updated = updated
        };

        sut.Actual.ShouldBe(expected);
    }
}
