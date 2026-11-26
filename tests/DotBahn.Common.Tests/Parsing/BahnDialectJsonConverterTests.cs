using DotBahn.Common.Parsing.Converters;
using Shouldly;
using System.Text.Json;

namespace DotBahn.Common.Tests.Parsing;

public class BahnDialectJsonConverterTests {
    private readonly JsonSerializerOptions _options = new() {
        Converters = {
            new BahnDialectJsonConverter()
        }
    };

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("\"TRUE\"", true)]
    [InlineData("\"FALSE\"", false)]
    [InlineData("\"YES\"", true)]
    [InlineData("\"NO\"", false)]
    [InlineData("\"JA\"", true)]
    [InlineData("\"1\"", true)]
    [InlineData("\"yes\"", true)]
    [InlineData("\"no\"", false)]
    [InlineData("\"maybe\"", false)]
    [InlineData("\"  TRUE  \"", true)]
    public void Read_Should_Parse_Dialect_Boolean_When_Input_Is_Valid(string json, bool expected) {
        var result = JsonSerializer.Deserialize<bool>(json, _options);

        result.ShouldBe(expected);
    }

    [Fact]
    public void Read_Should_Throw_JsonException_When_Token_Is_Number() {
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<bool>("42", _options));
    }

    [Fact]
    public void Write_Should_Serialize_True_As_Boolean_Token_When_Value_Is_True() {
        JsonSerializer.Serialize(true, _options).ShouldBe("true");
    }

    [Fact]
    public void Write_Should_Serialize_False_As_Boolean_Token_When_Value_Is_False() {
        JsonSerializer.Serialize(false, _options).ShouldBe("false");
    }
}
