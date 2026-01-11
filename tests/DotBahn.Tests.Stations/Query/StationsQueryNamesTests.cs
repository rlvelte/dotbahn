using DotBahn.Clients.Stations.Models;

namespace DotBahn.Tests.Stations.Query;

public class StationsQueryNamesTests {
    [Fact]
    public void Names_WithNullValue_ShouldThrowArgumentException() {
        // Arrange
        var query = new StationsQuery();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => query.Names = null);
        Assert.Equal("value", exception.ParamName);
        Assert.Contains("At least one name is required", exception.Message);
    }

    [Fact]
    public void Names_WithEmptyArray_ShouldThrowArgumentException() {
        // Arrange
        var query = new StationsQuery();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => query.Names = []);
        Assert.Equal("value", exception.ParamName);
    }

    [Theory]
    [InlineData("Hamburg", "Hamburg*")]
    [InlineData("Berlin", "Berlin*")]
    [InlineData("München", "München*")]
    public void Names_WithoutWildcard_ShouldAppendAsterisk(string input, string expected) {
        // Arrange & Act
        var query = new StationsQuery {
            Names = [input]
        };

        // Assert
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
        // Arrange & Act
        var query = new StationsQuery {
            Names = [nameWithWildcard]
        };

        // Assert
        Assert.Equal(nameWithWildcard, query.Names[0]);
    }

    [Fact]
    public void Names_WithMultipleValues_ShouldProcessEachIndividually() {
        // Arrange & Act
        var query = new StationsQuery {
            Names = ["Hamburg", "Berlin*", "München?"]
        };

        // Assert
        Assert.Equal(3, query.Names.Length);
        Assert.Equal("Hamburg*", query.Names[0]);
        Assert.Equal("Berlin*", query.Names[1]);
        Assert.Equal("München?", query.Names[2]);
    }

    [Fact]
    public void WithNames_ShouldSetNamesAndReturnQuery() {
        // Arrange
        var query = new StationsQuery();

        // Act
        var result = query.WithNames("Hamburg", "Berlin");

        // Assert
        Assert.Same(query, result);
        Assert.Equal(["Hamburg*", "Berlin*"], query.Names!);
    }

    [Fact]
    public void WithNames_FluentChaining_ShouldWork() {
        // Arrange & Act
        var query = new StationsQuery()
                    .WithNames("Hamburg")
                    .WithCategories("1")
                    .LimitTo(5);

        // Assert
        Assert.Equal(["Hamburg*"], query.Names!);
        Assert.Equal("1", query.Categories);
        Assert.Equal(5, query.Limit);
    }
}