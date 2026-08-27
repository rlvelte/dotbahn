using DotBahn.Stations;

namespace DotBahn.Tests.Stations.Query;

public class StationQueryCategoriesTests {
    [Fact]
    public void WithCategories_Null_ShouldThrowArgumentException() {
        var query = new StationQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.WithCategories(null!));
        Assert.Equal("categories", exception.ParamName);
        Assert.Contains("At least one category must be specified", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void WithCategories_Whitespace_ShouldThrowArgumentException(string whitespace) {
        var query = new StationQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.WithCategories(whitespace));
        Assert.Equal("categories", exception.ParamName);
    }

    [Theory]
    [InlineData("1", "1")]
    [InlineData("7", "7")]
    [InlineData("4", "4")]
    public void WithCategories_ValidSingle_SetsValue(string input, string expected) {
        var query = new StationQuery().WithCategories(input);

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("8")]
    [InlineData("10")]
    [InlineData("abc")]
    public void WithCategories_InvalidSingle_Throws(string invalid) {
        var query = new StationQuery();

        Assert.Throws<ArgumentException>(() => query.WithCategories(invalid));
    }

    [Theory]
    [InlineData("1-3", "1-3")]
    [InlineData("2-7", "2-7")]
    [InlineData("1-1", "1-1")]
    public void WithCategories_ValidRange_SetsValue(string input, string expected) {
        var query = new StationQuery().WithCategories(input);

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData("0-3")]
    [InlineData("1-8")]
    [InlineData("8-8")]
    [InlineData("5-4")]
    [InlineData("-1-3")]
    public void WithCategories_InvalidRange_Throws(string invalidRange) {
        var query = new StationQuery();

        Assert.Throws<ArgumentException>(() => query.WithCategories(invalidRange));
    }

    [Theory]
    [InlineData("a-3")]
    [InlineData("1-b")]
    [InlineData("x-y")]
    [InlineData("1--3")]
    public void WithCategories_NonNumericRange_Throws(string invalidRange) {
        var query = new StationQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.WithCategories(invalidRange));
        Assert.Contains("Invalid category range", exception.Message);
    }

    [Theory]
    [InlineData("1,3,5", "1,3,5")]
    [InlineData("1,2,3", "1,2,3")]
    [InlineData("7,6,5", "7,6,5")]
    public void WithCategories_MultipleValues_SetsValue(string input, string expected) {
        var query = new StationQuery().WithCategories(input);

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData("1,3-5,7", "1,3-5,7")]
    [InlineData("1-2,4,6-7", "1-2,4,6-7")]
    public void WithCategories_MixedCategoriesAndRanges_SetsValue(string input, string expected) {
        var query = new StationQuery().WithCategories(input);

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData(" 1 , 3 , 5 ", "1,3,5")]
    [InlineData("  1-3  ", "1-3")]
    [InlineData(" 1 - 3 ", "1-3")]
    public void WithCategories_Whitespace_TrimsAndNormalizes(string input, string expected) {
        var query = new StationQuery().WithCategories(input);

        Assert.Equal(expected, query.Categories);
    }

    [Fact]
    public void WithCategories_ReturnsSameQuery() {
        var query = new StationQuery();

        var result = query.WithCategories("1-3");

        Assert.Same(query, result);
        Assert.Equal("1-3", query.Categories);
    }

    [Fact]
    public void WithCategories_RangeAtBoundary_SetsValue() {
        var query = new StationQuery().WithCategories("7-7");

        Assert.Equal("7-7", query.Categories);
    }

    [Fact]
    public void WithCategoriesRangeOverflowThrowsOverflowException() {
        var query = new StationQuery();

        Assert.Throws<OverflowException>(() => query.WithCategories("9999999999999-1"));
    }

    [Fact]
    public void Categories_SetDirectly_RawValue() {
        var query = new StationQuery { Categories = "1-3" };

        Assert.Equal("1-3", query.Categories);
    }

    public static TheoryData<string, string, bool> WithCategoriesRangeBoundaryCases => new()
    {
        { "BL—all_valid", "2-6", false },
        { "C1—start_lt_1", "0-3", true },
        { "C2—start_gt_7", "8-8", true },
        { "C4—end_gt_7", "1-8", true },
        { "C5—start_gt_end", "3-1", true },
    };

    [Theory]
    [MemberData(nameof(WithCategoriesRangeBoundaryCases))]
    public void WithCategoriesRangeBoundary(string _, string categories, bool shouldThrow) {
        var query = new StationQuery();
        if (shouldThrow)
            Assert.Throws<ArgumentException>(() => query.WithCategories(categories));
        else
            Assert.Equal(categories, query.WithCategories(categories).Categories);
    }
}
