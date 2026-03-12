using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Modules.Shared.Parsing;

namespace DotBahn.Tests.Timetables.Parsing;

public class JsonParserTests {
    private readonly JsonParser<TimetableResponseContract> _parser = new();

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
    public void Parse_ValidJson_ReturnsPopulatedContract() {
        // Arrange
        var json = """{"station":"Hamburg Hbf","stops":[]}""";

        // Act
        var result = _parser.Parse(json);

        // Assert
        Assert.Equal("Hamburg Hbf", result.Station);
    }

    [Fact]
    public void Parse_JsonWithCaseInsensitiveKeys_ReturnsPopulatedContract() {
        // Arrange
        var json = """{"STATION":"Hamburg Hbf","stops":[]}""";

        // Act
        var result = _parser.Parse(json);

        // Assert
        Assert.Equal("Hamburg Hbf", result.Station);
    }
}
