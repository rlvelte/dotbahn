using DotBahn.Stations;
using DotBahn.Stations.Models.Enumerations;
using Shouldly;

namespace DotBahn.Tests.Stations.Query;

public class StationQueryTests {
    [Fact]
    public void Constructor_Should_UseDocumentedDefaults_When_Called() {
        var sut = new StationQuery();

        sut.Names.ShouldBeNull();
        sut.Categories.ShouldBeNull();
        sut.State.ShouldBeNull();
        sut.Eva.ShouldBeNull();
        sut.Ril.ShouldBeNull();
        sut.Operator.ShouldBe(LogicalOperator.And);
        sut.Offset.ShouldBe(0);
        sut.Limit.ShouldBe(10000);
    }

    [Theory]
    [InlineData("Hamburg", "Hamburg*")]
    [InlineData("München", "München*")]
    [InlineData("Ham*", "Ham*")]
    [InlineData("H?mb", "H?mb")]
    [InlineData("*", "*")]
    [InlineData("", "*")]
    [InlineData("   ", "   *")]
    public void WithNames_Should_AppendWildcard_When_PatternHasNoWildcard(string name, string expected) {
        var sut = new StationQuery();

        var result = sut.WithNames(name);

        result.ShouldBeSameAs(sut);
        sut.Names.ShouldBe([expected]);
    }

    [Fact]
    public void WithNames_Should_ThrowArgumentException_When_NamesAreNull() {
        var sut = new StationQuery();

        var exception = Should.Throw<ArgumentException>(() => sut.WithNames(null!));

        exception.ParamName.ShouldBe("names");
        exception.Message.ShouldContain("At least one name is required.");
    }

    [Fact]
    public void WithNames_Should_ThrowArgumentException_When_NamesAreEmpty() {
        var sut = new StationQuery();

        var exception = Should.Throw<ArgumentException>(() => sut.WithNames([]));

        exception.ParamName.ShouldBe("names");
        exception.Message.ShouldContain("At least one name is required.");
    }

    [Theory]
    [InlineData("4", "4")]
    [InlineData("1", "1")]
    [InlineData("7", "7")]
    [InlineData("2-4", "2-4")]
    [InlineData("1 - 7", "1-7")]
    [InlineData("2 - 4", "2-4")]
    [InlineData("1,3-5,7", "1,3-5,7")]
    [InlineData(" 2 , 4 ", "2,4")]
    [InlineData("1,,2", "1,2")]
    [InlineData("1-1", "1-1")]
    [InlineData("7-7", "7-7")]
    public void WithCategories_Should_Normalize_When_InputIsValid(string input, string expected) {
        var sut = new StationQuery();

        var result = sut.WithCategories(input);

        result.ShouldBeSameAs(sut);
        sut.Categories.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, "At least one category must be specified.")]
    [InlineData("", "At least one category must be specified.")]
    [InlineData("   ", "At least one category must be specified.")]
    [InlineData("0", "Category must be between 1 and 7")]
    [InlineData("8", "Category must be between 1 and 7")]
    [InlineData("-1", "Invalid category range")]
    [InlineData("abc", "Category must be between 1 and 7")]
    [InlineData("1.5", "Category must be between 1 and 7")]
    [InlineData("0-3", "Category range out of bounds")]
    [InlineData("3-8", "Category range out of bounds")]
    [InlineData("5-2", "Category range out of bounds")]
    [InlineData("0-0", "Category range out of bounds")]
    [InlineData("8-8", "Category range out of bounds")]
    [InlineData("2-", "Invalid category range")]
    [InlineData("a-b", "Invalid category range")]
    [InlineData("1-2-3", "Invalid category range")]
    [InlineData("1 - x", "Invalid category range")]
    public void WithCategories_Should_ThrowArgumentException_When_InputIsInvalid(string? input, string expectedMessage) {
        var sut = new StationQuery();

        var exception = Should.Throw<ArgumentException>(() => sut.WithCategories(input!));

        exception.ParamName.ShouldBe("categories");
        exception.Message.ShouldContain(expectedMessage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5000)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Skip_Should_SetOffset_When_ValueIsBoundaryOrInterior(int value) {
        var sut = new StationQuery();

        var result = sut.Skip(value);

        result.ShouldBeSameAs(sut);
        sut.Offset.ShouldBe(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9999)]
    [InlineData(10000)]
    [InlineData(10001)]
    [InlineData(int.MaxValue)]
    public void LimitTo_Should_SetLimit_When_ValueIsBoundaryOrInterior(int value) {
        var sut = new StationQuery();

        var result = sut.LimitTo(value);

        result.ShouldBeSameAs(sut);
        sut.Limit.ShouldBe(value);
    }

    [Theory]
    [InlineData(LogicalOperator.And)]
    [InlineData(LogicalOperator.Or)]
    public void CombineAs_Should_SetOperator_When_ValueIsProvided(LogicalOperator value) {
        var sut = new StationQuery();

        var result = sut.CombineAs(value);

        result.ShouldBeSameAs(sut);
        sut.Operator.ShouldBe(value);
    }

    [Theory]
    [InlineData(FederalState.Berlin)]
    [InlineData(FederalState.BadenWuerttemberg)]
    [InlineData(FederalState.NorthRhineWestphalia)]
    [InlineData(FederalState.Unknown)]
    public void InFederalState_Should_SetState_When_ValueIsProvided(FederalState value) {
        var sut = new StationQuery();

        var result = sut.InFederalState(value);

        result.ShouldBeSameAs(sut);
        sut.State.ShouldBe(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("8002549")]
    public void WithEva_Should_SetEva_When_ValueIsProvided(string value) {
        var sut = new StationQuery();

        var result = sut.WithEva(value);

        result.ShouldBeSameAs(sut);
        sut.Eva.ShouldBe(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("AH")]
    public void WithRil_Should_SetRil_When_ValueIsProvided(string value) {
        var sut = new StationQuery();

        var result = sut.WithRil(value);

        result.ShouldBeSameAs(sut);
        sut.Ril.ShouldBe(value);
    }
}
