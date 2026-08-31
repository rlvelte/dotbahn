using DotBahn.Timetables.Models;
using DotBahn.Timetables.Models.Enumerations;

namespace DotBahn.Timetables.Tests.Models;

public class TimetableMessageTests {
    private static TimetableMessage Make(bool isDeleted = false, DateTime? validTo = null, string? internalText = null, string? externalText = null) => new() {
        Id = "m1",
        Type = MessageType.Him,
        Timestamp = DateTime.UtcNow,
        IsDeleted = isDeleted,
        ValidTo = validTo,
        InternalText = internalText,
        ExternalText = externalText
    };

    [Theory]
    [InlineData(null, "public info", "public info")]
    [InlineData("internal info", null, "internal info")]
    [InlineData("internal", "external", "external")]
    [InlineData(null, null, null)]
    public void Text_ReturnsExpected(string? internalText, string? externalText, string? expected) {
        var msg = Make(internalText: internalText, externalText: externalText);

        Assert.Equal(expected, msg.Text);
    }

    [Fact]
    public void Text_NeitherSet_ReturnsNull() {
        var msg = Make();

        Assert.Null(msg.Text);
    }

    [Fact]
    public void IsActive_NotDeletedAndNoValidTo_ReturnsTrue() {
        var msg = Make(isDeleted: false, validTo: null);

        Assert.True(msg.IsActive);
    }

    [Fact]
    public void IsActive_NotDeletedAndValidToInFuture_ReturnsTrue() {
        var msg = Make(isDeleted: false, validTo: DateTime.Now.AddHours(1));

        Assert.True(msg.IsActive);
    }

    [Fact]
    public void IsActive_NotDeletedAndValidToInPast_ReturnsFalse() {
        var msg = Make(isDeleted: false, validTo: DateTime.Now.AddHours(-1));

        Assert.False(msg.IsActive);
    }

    [Fact]
    public void IsActive_Deleted_ReturnsFalse() {
        var msg = Make(isDeleted: true, validTo: null);

        Assert.False(msg.IsActive);
    }

    [Fact]
    public void IsActive_DeletedAndValidToInFuture_ReturnsFalse() {
        var msg = Make(isDeleted: true, validTo: DateTime.Now.AddHours(1));

        Assert.False(msg.IsActive);
    }

    [Fact]
    public void IsActive_WithValidToExactlyNow_ReturnsFalse() {
        var msg = Make(isDeleted: false, validTo: DateTime.Now);

        Assert.False(msg.IsActive);
    }
}
