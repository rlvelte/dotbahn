using DotBahn.Common.Models;

namespace DotBahn.Tests.Timetables.Models;

public class ChangedRefTests {
    [Theory]
    [InlineData("original", null, "original", false)]
    [InlineData("original", "updated", "updated", true)]
    [InlineData("x", null, "x", false)]
    public void Actual_ReturnsExpectedValue(string original, string? updated, string expected, bool hasUpdate) {
        var changed = new ChangedRef<string> {
            Original = original,
            Updated = updated
        };

        Assert.Equal(expected, changed.Actual);
        Assert.Equal(hasUpdate, changed.HasUpdate);
    }
}
