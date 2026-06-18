using DotBahn.Shared.Parsing;
using DotBahn.Timetables;
using DotBahn.Timetables.Contracts;

namespace DotBahn.Tests.Timetables.Parsing;

public class XmlParserTests {
    private readonly XmlParser<TimetableResponseContract> _parser = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyOrWhitespace_ReturnsDefaultInstance(string input) {
        var result = _parser.Parse(input);

        Assert.NotNull(result);
        Assert.Empty(result.Stops);
    }

    [Fact]
    public void Parse_ValidXml_ReturnsPopulatedContract() {
        var xml = """<?xml version="1.0"?><timetable station="München Hbf" />""";

        var result = _parser.Parse(xml);

        Assert.Equal("München Hbf", result.Station);
    }

    [Fact]
    public void Parse_ValidXmlWithStop_ReturnsStops() {
        var xml = """<?xml version="1.0"?><timetable station="Berlin Hbf"><s id="stop-1"><tl c="ICE" n="100" o="80" /></s></timetable>""";

        var result = _parser.Parse(xml);

        Assert.Equal("Berlin Hbf", result.Station);
        Assert.Single(result.Stops);
        Assert.Equal("stop-1", result.Stops[0].Id);
    }
}
