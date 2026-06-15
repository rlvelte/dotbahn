using DotBahn.Data.Timetables.Enumerations;
using DotBahn.Data.Timetables.Models;

namespace DotBahn.Tests.Timetables.Models;

public class TimetableMessageTests {
    private static TimetableMessage Make(
        bool isDeleted = false,
        DateTime? validTo = null,
        string? internalText = null,
        string? externalText = null) => new() {
            Id = "m1",
            Type = MessageType.Him,
            Timestamp = DateTime.UtcNow,
            IsDeleted = isDeleted,
            ValidTo = validTo,
            InternalText = internalText,
            ExternalText = externalText
        };

    [Fact]
    public void Text_OnlyExternalText_ReturnsExternal() {
        // Arrange
        var msg = Make(externalText: "public info");

        // Assert
        Assert.Equal("public info", msg.Text);
    }

    [Fact]
    public void Text_OnlyInternalText_ReturnsInternal() {
        // Arrange
        var msg = Make(internalText: "internal info");

        // Assert
        Assert.Equal("internal info", msg.Text);
    }

    [Fact]
    public void Text_BothSet_PrefersExternalText() {
        // Arrange
        var msg = Make(internalText: "internal", externalText: "external");

        // Assert
        Assert.Equal("external", msg.Text);
    }

    [Fact]
    public void Text_NeitherSet_ReturnsNull() {
        // Arrange
        var msg = Make();

        // Assert
        Assert.Null(msg.Text);
    }

    [Fact]
    public void IsActive_NotDeletedAndNoValidTo_ReturnsTrue() {
        // Arrange
        var msg = Make(isDeleted: false, validTo: null);

        // Assert
        Assert.True(msg.IsActive);
    }

    [Fact]
    public void IsActive_NotDeletedAndValidToInFuture_ReturnsTrue() {
        // Arrange
        var msg = Make(isDeleted: false, validTo: DateTime.Now.AddHours(1));

        // Assert
        Assert.True(msg.IsActive);
    }

    [Fact]
    public void IsActive_NotDeletedAndValidToInPast_ReturnsFalse() {
        // Arrange
        var msg = Make(isDeleted: false, validTo: DateTime.Now.AddHours(-1));

        // Assert
        Assert.False(msg.IsActive);
    }

    [Fact]
    public void IsActive_Deleted_ReturnsFalse() {
        // Arrange
        var msg = Make(isDeleted: true, validTo: null);

        // Assert
        Assert.False(msg.IsActive);
    }

    [Fact]
    public void IsActive_DeletedAndValidToInFuture_ReturnsFalse() {
        // Arrange
        var msg = Make(isDeleted: true, validTo: DateTime.Now.AddHours(1));

        // Assert
        Assert.False(msg.IsActive);
    }
}
