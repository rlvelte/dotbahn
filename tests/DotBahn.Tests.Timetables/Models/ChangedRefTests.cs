using DotBahn.Shared.Models;

namespace DotBahn.Tests.Timetables.Models;

public class ChangedRefTests {
    [Theory]
    [InlineData("original", null, "original", false)]  // no update → original
    [InlineData("original", "updated", "updated", true)] // has update → updated
    [InlineData("x", null, "x", false)]                 // null updated → falls back to original
    public void Actual_ReturnsExpectedValue(string original, string? updated, string expected, bool hasUpdate) {
        var changed = new ChangedRef<string> { Original = original, Updated = updated };

        Assert.Equal(expected, changed.Actual);
        Assert.Equal(hasUpdate, changed.HasUpdate);
    }
}
