using DotBahn.Data.Shared.Models;

namespace DotBahn.Tests.Timetables.Models;

public class ChangedRefTests {
    [Fact]
    public void Actual_WithNoUpdate_ReturnsOriginal() {
        // Arrange
        var changed = new ChangedRef<string> { Original = "original" };

        // Assert
        Assert.Equal("original", changed.Actual);
        Assert.False(changed.HasUpdate);
    }

    [Fact]
    public void Actual_WithUpdate_ReturnsUpdated() {
        // Arrange
        var changed = new ChangedRef<string> { Original = "original", Updated = "updated" };

        // Assert
        Assert.Equal("updated", changed.Actual);
        Assert.True(changed.HasUpdate);
    }

    [Fact]
    public void HasUpdate_WhenUpdatedIsNull_ReturnsFalse() {
        // Arrange
        var changed = new ChangedRef<string> { Original = "x", Updated = null };

        // Assert
        Assert.False(changed.HasUpdate);
    }
}
