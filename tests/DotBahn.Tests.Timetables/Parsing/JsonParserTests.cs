using DotBahn.Common.Parsing;
using DotBahn.Timetables.Internal.Contracts;

namespace DotBahn.Tests.Timetables.Parsing;

public class JsonParserTests {
    private readonly JsonParser<TimetableResponseContract> _parser = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyOrWhitespace_ReturnsDefaultInstance(string input) {
        var result = _parser.Parse(input);

        Assert.NotNull(result);
        Assert.Empty(result.Stops);
    }

    [Fact]
    public void Parse_ValidJson_ReturnsPopulatedContract() {
        var json = """{"station":"Hamburg Hbf","stops":[]}""";

        var result = _parser.Parse(json);

        Assert.Equal("Hamburg Hbf", result.Station);
    }

    [Fact]
    public void Parse_JsonWithCaseInsensitiveKeys_ReturnsPopulatedContract() {
        var json = """{"STATION":"Hamburg Hbf","stops":[]}""";

        var result = _parser.Parse(json);

        Assert.Equal("Hamburg Hbf", result.Station);
    }
}
