using DotBahn.Common.Utilities;

namespace DotBahn.Tests.Timetables.Client;

public class QueryParametersTests {

    [Fact]
    public void Add_WithNullStringValue_ShouldNotCreateEntry() {
        var parameters = QueryParameters.Create();

        parameters.Add("key", null);

        Assert.False(parameters.Any());
        Assert.Equal(string.Empty, parameters.ToQueryString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Add_WithEmptyOrWhitespaceValue_ShouldNotCreateEntry(string value) {
        var parameters = QueryParameters.Create();

        parameters.Add("key", value);

        Assert.False(parameters.Any());
    }

    [Fact]
    public void Add_WithNullCollection_ShouldNotCreateEntry() {
        var parameters = QueryParameters.Create();

        parameters.Add<string>("key", null);

        Assert.False(parameters.Any());
    }

    [Fact]
    public void Add_WithEmptyCollection_ShouldNotCreateEntry() {
        var parameters = QueryParameters.Create();

        parameters.Add("key", Array.Empty<string>());

        Assert.False(parameters.Any());
    }

    [Fact]
    public void Add_WithNonEmptyCollection_ShouldCreateCommaSeparatedEntry() {
        var parameters = QueryParameters.Create();

        parameters.Add("key", ["a", "b"]);

        Assert.True(parameters.Any());
        var queryString = parameters.ToQueryString();
        Assert.Contains("key=a%2Cb", queryString);
        Assert.Contains("a%2Cb", queryString);
    }
}
