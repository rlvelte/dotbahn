using DotBahn.Stations;

namespace DotBahn.Tests.Stations.Query;

public class StationQueryNamesTests {
    [Fact]
    public void Names_WithNullValue_ShouldThrowArgumentException() {
        var query = new StationQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.Names = null);
        Assert.Equal("value", exception.ParamName);
        Assert.Contains("At least one name is required", exception.Message);
    }

    [Fact]
    public void Names_WithEmptyArray_ShouldThrowArgumentException() {
        var query = new StationQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.Names = []);
        Assert.Equal("value", exception.ParamName);
    }

    [Theory]
    [InlineData("Hamburg", "Hamburg*")]
    [InlineData("Berlin", "Berlin*")]
    [InlineData("München", "München*")]
    public void Names_WithoutWildcard_ShouldAppendAsterisk(string input, string expected) {
        var query = new StationQuery {
            Names = [input]
        };

        Assert.Single(query.Names);
        Assert.Equal(expected, query.Names[0]);
    }

    [Theory]
    [InlineData("Hamburg*")]
    [InlineData("*Hamburg")]
    [InlineData("Ham*burg")]
    [InlineData("Hamburg?")]
    [InlineData("?Hamburg")]
    [InlineData("Ham?burg")]
    [InlineData("*Hamburg?")]
    public void Names_WithWildcard_ShouldNotAppendAsterisk(string nameWithWildcard) {
        var query = new StationQuery {
            Names = [nameWithWildcard]
        };

        Assert.Equal(nameWithWildcard, query.Names[0]);
    }

    [Fact]
    public void Names_WithMultipleValues_ShouldProcessEachIndividually() {
        var query = new StationQuery {
            Names = ["Hamburg", "Berlin*", "München?"]
        };

        Assert.Equal(3, query.Names.Length);
        Assert.Equal("Hamburg*", query.Names[0]);
        Assert.Equal("Berlin*", query.Names[1]);
        Assert.Equal("München?", query.Names[2]);
    }

    [Fact]
    public void WithNames_ShouldSetNamesAndReturnQuery() {
        var query = new StationQuery();

        var result = query.WithNames("Hamburg", "Berlin");

        Assert.Same(query, result);
        Assert.Equal(["Hamburg*", "Berlin*"], query.Names!);
    }

    [Fact]
    public void WithNames_FluentChaining_ShouldWork() {
        var query = new StationQuery()
                    .WithNames("Hamburg")
                    .WithCategories("1")
                    .LimitTo(5);

        Assert.Equal(["Hamburg*"], query.Names!);
        Assert.Equal("1", query.Categories);
        Assert.Equal(5, query.Limit);
    }
}
