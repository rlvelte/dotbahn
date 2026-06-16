using System.Text.Json;

using DotBahn.Clients.Shared.Parsing.Additional;

namespace DotBahn.Tests.Timetables.Parsing;

public class BahnDialectParserTests {
    private readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true,
        Converters = { new BahnDialectJsonConverter() }
    };

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Read_BooleanLiteral_ReturnsCorrectValue(string json, bool expected) {
        var result = JsonSerializer.Deserialize<bool>(json, _options);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"true\"")]
    [InlineData("\"TRUE\"")]
    [InlineData("\"yes\"")]
    [InlineData("\"YES\"")]
    [InlineData("\"1\"")]
    public void Read_TruthyString_ReturnsTrue(string json) {
        var result = JsonSerializer.Deserialize<bool>(json, _options);

        Assert.True(result);
    }

    [Theory]
    [InlineData("\"false\"")]
    [InlineData("\"FALSE\"")]
    [InlineData("\"no\"")]
    [InlineData("\"NO\"")]
    [InlineData("\"0\"")]
    [InlineData("\"\"")]
    [InlineData("\"anything\"")]
    public void Read_FalsyString_ReturnsFalse(string json) {
        var result = JsonSerializer.Deserialize<bool>(json, _options);

        Assert.False(result);
    }

    [Fact]
    public void Read_UnexpectedTokenType_ThrowsJsonException() =>
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<bool>("42", _options));

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Write_BooleanValue_WritesCorrectJson(bool value, string expected) {
        var result = JsonSerializer.Serialize(value, _options);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Write_ThenRead_RoundTripsBooleanValue(bool value) {
        var json = JsonSerializer.Serialize(value, _options);
        var result = JsonSerializer.Deserialize<bool>(json, _options);

        Assert.Equal(value, result);
    }
}
