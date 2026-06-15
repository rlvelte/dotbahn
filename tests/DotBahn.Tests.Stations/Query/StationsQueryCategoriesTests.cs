using DotBahn.Clients.Stations.Query;

namespace DotBahn.Tests.Stations.Query;

public class StationsQueryCategoriesTests {
    [Fact]
    public void Categories_WithNullValue_ShouldThrowArgumentException() {
        var query = new StationsQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.Categories = null);
        Assert.Equal("value", exception.ParamName);
        Assert.Contains("At least one category must be specified", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Categories_WithWhitespace_ShouldThrowArgumentException(string whitespace) {
        var query = new StationsQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.Categories = whitespace);
        Assert.Equal("value", exception.ParamName);
    }

    [Theory]
    [InlineData("1", "1")]
    [InlineData("7", "7")]
    [InlineData("4", "4")]
    public void Categories_WithValidSingleCategory_ShouldSetValue(string input, string expected) {
        var query = new StationsQuery {
            Categories = input
        };

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("8")]
    [InlineData("10")]
    public void Categories_WithInvalidSingleCategory_ShouldThrowArgumentException(string invalid) {
        var query = new StationsQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.Categories = invalid);
        Assert.Contains("must be between 1 and 7", exception.Message);
    }

    [Theory]
    [InlineData("1-3", "1-3")]
    [InlineData("2-7", "2-7")]
    [InlineData("1-1", "1-1")]
    public void Categories_WithValidRange_ShouldSetValue(string input, string expected) {
        var query = new StationsQuery {
            Categories = input
        };

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData("0-3")]
    [InlineData("1-8")]
    [InlineData("8-8")]
    [InlineData("5-4")]
    [InlineData("-1-3")]
    public void Categories_WithInvalidRange_ShouldThrowArgumentException(string invalidRange) {
        var query = new StationsQuery();

        Assert.Throws<ArgumentException>(() => query.Categories = invalidRange);
    }

    [Theory]
    [InlineData("a-3")]
    [InlineData("1-b")]
    [InlineData("x-y")]
    [InlineData("1--3")]
    public void Categories_WithNonNumericRange_ShouldThrowArgumentException(string invalidRange) {
        var query = new StationsQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.Categories = invalidRange);
        Assert.Contains("Invalid category range", exception.Message);
    }

    [Theory]
    [InlineData("1,3,5", "1,3,5")]
    [InlineData("1,2,3", "1,2,3")]
    [InlineData("7,6,5", "7,6,5")]
    public void Categories_WithMultipleCategories_ShouldSetValue(string input, string expected) {
        var query = new StationsQuery {
            Categories = input
        };

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData("1,3-5,7", "1,3-5,7")]
    [InlineData("1-2,4,6-7", "1-2,4,6-7")]
    public void Categories_WithMixedCategoriesAndRanges_ShouldSetValue(string input, string expected) {
        var query = new StationsQuery {
            Categories = input
        };

        Assert.Equal(expected, query.Categories);
    }

    [Theory]
    [InlineData(" 1 , 3 , 5 ", "1,3,5")]
    [InlineData("  1-3  ", "1-3")]
    [InlineData(" 1 - 3 ", "1-3")]
    public void Categories_WithWhitespace_ShouldTrimAndNormalize(string input, string expected) {
        var query = new StationsQuery {
            Categories = input
        };

        Assert.Equal(expected, query.Categories);
    }

    [Fact]
    public void WithCategories_WithSingleParameter_ShouldSetCategories() {
        var query = new StationsQuery();

        var result = query.WithCategories("1-3");

        Assert.Same(query, result);
        Assert.Equal("1-3", query.Categories);
    }

    [Fact]
    public void Categories_WithRangeStartAtBoundary_ShouldSetValue() {
        var query = new StationsQuery {
            Categories = "7-7"
        };

        Assert.Equal("7-7", query.Categories);
    }
}
