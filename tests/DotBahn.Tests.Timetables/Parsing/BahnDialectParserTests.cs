using System.Text.Json;
using DotBahn.Modules.Shared.Parsing.Additional;

namespace DotBahn.Tests.Timetables.Parsing;

public class BahnDialectParserTests {
    private readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true,
        Converters = { new BahnDialectJsonConverter() }
    };

    [Fact]
    public void Read_JsonTrue_ReturnsTrue() {
        // Act
        var result = JsonSerializer.Deserialize<bool>("true", _options);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Read_JsonFalse_ReturnsFalse() {
        // Act
        var result = JsonSerializer.Deserialize<bool>("false", _options);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("\"true\"")]
    [InlineData("\"TRUE\"")]
    [InlineData("\"yes\"")]
    [InlineData("\"YES\"")]
    [InlineData("\"1\"")]
    public void Read_TruthyString_ReturnsTrue(string json) {
        // Act
        var result = JsonSerializer.Deserialize<bool>(json, _options);

        // Assert
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
        // Act
        var result = JsonSerializer.Deserialize<bool>(json, _options);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Read_UnexpectedTokenType_ThrowsJsonException() {
        // Act + Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<bool>("42", _options));
    }

    [Fact]
    public void Write_TrueValue_WritesJsonTrue() {
        // Act
        var result = JsonSerializer.Serialize(true, _options);

        // Assert
        Assert.Equal("true", result);
    }

    [Fact]
    public void Write_FalseValue_WritesJsonFalse() {
        // Act
        var result = JsonSerializer.Serialize(false, _options);

        // Assert
        Assert.Equal("false", result);
    }
}
