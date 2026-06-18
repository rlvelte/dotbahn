using DotBahn.Stations;

namespace DotBahn.Tests.Stations.Query;

public class StationQueryNamesTests {
    [Fact]
    public void WithNames_Null_ShouldThrowArgumentException() {
        var query = new StationQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.WithNames(null!));
        Assert.Equal("names", exception.ParamName);
        Assert.Contains("At least one name is required", exception.Message);
    }

    [Fact]
    public void WithNames_Empty_ShouldThrowArgumentException() {
        var query = new StationQuery();

        var exception = Assert.Throws<ArgumentException>(() => query.WithNames());
        Assert.Equal("names", exception.ParamName);
    }

    [Theory]
    [InlineData("Hamburg", "Hamburg*")]
    [InlineData("Berlin", "Berlin*")]
    [InlineData("München", "München*")]
    public void WithNames_WithoutWildcard_ShouldAppendAsterisk(string input, string expected) {
        var query = new StationQuery().WithNames(input);

        Assert.Single(query.Names!);
        Assert.Equal(expected, query.Names![0]);
    }

    [Theory]
    [InlineData("Hamburg*")]
    [InlineData("*Hamburg")]
    [InlineData("Ham*burg")]
    [InlineData("Hamburg?")]
    [InlineData("?Hamburg")]
    [InlineData("Ham?burg")]
    [InlineData("*Hamburg?")]
    public void WithNames_WithWildcard_ShouldNotAppendAsterisk(string nameWithWildcard) {
        var query = new StationQuery().WithNames(nameWithWildcard);

        Assert.Equal(nameWithWildcard, query.Names![0]);
    }

    [Fact]
    public void WithNames_WithMultipleValues_ShouldProcessEachIndividually() {
        var query = new StationQuery().WithNames("Hamburg", "Berlin*", "München?");

        Assert.Equal(3, query.Names!.Length);
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

    [Theory]
    [InlineData("Hamburg")]
    [InlineData("Berlin")]
    public void Names_SetDirectly_RawValues(string input) {
        var query = new StationQuery { Names = [input] };

        Assert.Equal([input], query.Names!);
    }
}
