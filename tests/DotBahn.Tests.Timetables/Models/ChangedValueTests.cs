using DotBahn.Data.Shared.Models;

namespace DotBahn.Tests.Timetables.Models;

public class ChangedValueTests {
    [Fact]
    public void Actual_WithNoUpdate_ReturnsOriginal() {
        // Arrange
        var changed = new ChangedValue<int> { Original = 42 };

        // Assert
        Assert.Equal(42, changed.Actual);
        Assert.False(changed.HasUpdate);
    }

    [Fact]
    public void Actual_WithUpdate_ReturnsUpdated() {
        // Arrange
        var changed = new ChangedValue<int> { Original = 42, Updated = 99 };

        // Assert
        Assert.Equal(99, changed.Actual);
        Assert.True(changed.HasUpdate);
    }

    [Fact]
    public void HasUpdate_WhenUpdatedIsNull_ReturnsFalse() {
        // Arrange
        var changed = new ChangedValue<DateTime> { Original = DateTime.MinValue, Updated = null };

        // Assert
        Assert.False(changed.HasUpdate);
    }
}
