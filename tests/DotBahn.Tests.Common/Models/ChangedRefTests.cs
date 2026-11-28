using DotBahn.Common.Models;
using Shouldly;

namespace DotBahn.Tests.Common.Models;

public class ChangedRefTests {
    [Fact]
    public void Actual_Should_ReturnOriginal_When_UpdatedIsNull() {
        var sut = new ChangedRef<string> {
            Original = "original"
        };

        sut.Actual.ShouldBe("original");
    }

    [Fact]
    public void Actual_Should_ReturnUpdated_When_UpdatedIsSet() {
        var sut = new ChangedRef<string> {
            Original = "original",
            Updated = "updated"
        };

        sut.Actual.ShouldBe("updated");
    }

    [Fact]
    public void HasUpdate_Should_BeFalse_When_UpdatedIsNull() {
        var sut = new ChangedRef<string> {
            Original = "original"
        };

        sut.HasUpdate.ShouldBeFalse();
    }

    [Fact]
    public void HasUpdate_Should_BeTrue_When_UpdatedIsSet() {
        var sut = new ChangedRef<string> {
            Original = "original",
            Updated = "updated"
        };

        sut.HasUpdate.ShouldBeTrue();
    }

    [Fact]
    public void HasUpdate_Should_BeTrue_When_UpdatedIsEmptyString() {
        var sut = new ChangedRef<string> {
            Original = "original",
            Updated = string.Empty
        };

        sut.HasUpdate.ShouldBeTrue();
        sut.Actual.ShouldBeEmpty();
    }

    [Fact]
    public void Actual_Should_ReturnUpdated_When_UpdatedIsReferenceTypeInstance() {
        var originalList = new List<int> { 1, 2 };
        var updatedList = new List<int> { 3, 4 };
        var sut = new ChangedRef<List<int>> {
            Original = originalList,
            Updated = updatedList
        };

        sut.Actual.ShouldBe(updatedList);
        sut.HasUpdate.ShouldBeTrue();
    }
}
