using DotBahn.Timetables.Internal.Parsing;

namespace DotBahn.Timetables.Tests.Parsing;

public class XmlParserTests {
    private readonly TimetableXmlParser _parser = new();

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
        const string xml = """<?xml version="1.0"?><timetable station="München Hbf" />""";

        var result = _parser.Parse(xml);

        Assert.Equal("München Hbf", result.Station);
    }

    public static TheoryData<string, string, string> ParseRootRequiresTimetableTagCases => new()
    {
        { "BL—valid_tag", """<?xml version="1.0"?><timetable station="Frankfurt Hbf"/>""", "Frankfurt Hbf" },
        { "C2—wrong_tag", """<?xml version="1.0"?><not-timetable station="Frankfurt Hbf"/>""", "" },
    };

    [Theory]
    [MemberData(nameof(ParseRootRequiresTimetableTagCases))]
    public void ParseRootRequiresTimetableTag(string _, string xml, string expectedStation) {
        var result = _parser.Parse(xml);
        Assert.Equal(expectedStation, result.Station);
    }

    [Fact]
    public void ParseStopMissingIdThrowsXmlException() {
        var xml = """<?xml version="1.0"?><timetable station="Test"><s eva="8000105"/></timetable>""";

        Assert.Throws<System.Xml.XmlException>(() => _parser.Parse(xml));
    }

    [Fact]
    public void ParseStopMissingEvaDefaultsToEmpty() {
        var xml = """<?xml version="1.0"?><timetable station="Test"><s id="s1"/></timetable>""";

        var result = _parser.Parse(xml);

        Assert.Equal("", result.Stops[0].Eva);
    }

    [Fact]
    public void Parse_ValidXmlWithStop_ReturnsStops() {
        const string xml = """<?xml version="1.0"?><timetable station="Berlin Hbf"><s id="stop-1" eva="8011160"><tl c="ICE" n="100" o="80" /></s></timetable>""";

        var result = _parser.Parse(xml);

        Assert.Equal("Berlin Hbf", result.Station);
        Assert.Single(result.Stops);
        Assert.Equal("stop-1", result.Stops[0].Id);
    }
}
