using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Tests.Timetables.Parsing;

public class XmlParserTests {
    private readonly XmlParser<TimetableResponseContract> _parser = new();

    [Fact]
    public void Parse_EmptyString_ReturnsDefaultInstance() {
        // Act
        var result = _parser.Parse(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Stops);
    }

    [Fact]
    public void Parse_WhitespaceString_ReturnsDefaultInstance() {
        // Act
        var result = _parser.Parse("   ");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Stops);
    }

    [Fact]
    public void Parse_ValidXml_ReturnsPopulatedContract() {
        // Arrange
        var xml = """<?xml version="1.0"?><timetable station="München Hbf" />""";

        // Act
        var result = _parser.Parse(xml);

        // Assert
        Assert.Equal("München Hbf", result.Station);
    }

    [Fact]
    public void Parse_ValidXmlWithStop_ReturnsStops() {
        // Arrange
        var xml = """<?xml version="1.0"?><timetable station="Berlin Hbf"><s id="stop-1"><tl c="ICE" n="100" o="80" /></s></timetable>""";

        // Act
        var result = _parser.Parse(xml);

        // Assert
        Assert.Equal("Berlin Hbf", result.Station);
        Assert.Single(result.Stops);
        Assert.Equal("stop-1", result.Stops[0].Id);
    }
}
