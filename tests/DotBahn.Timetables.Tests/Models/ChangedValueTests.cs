using DotBahn.Common.Models;

namespace DotBahn.Timetables.Tests.Models;

public class ChangedValueTests {
    [Theory]
    [InlineData(42, null, 42, false)]
    [InlineData(42, 99, 99, true)]
    public void Actual_ReturnsExpectedValue(int original, int? updated, int expected, bool hasUpdate) {
        var changed = new ChangedValue<int> {
            Original = original,
            Updated = updated
        };

        Assert.Equal(expected, changed.Actual);
        Assert.Equal(hasUpdate, changed.HasUpdate);
    }

    [Fact]
    public void HasUpdate_WhenUpdatedIsNull_ReturnsFalse() {
        var changed = new ChangedValue<DateTime> {
            Original = DateTime.MinValue,
            Updated = null
        };

        Assert.False(changed.HasUpdate);
    }
}
