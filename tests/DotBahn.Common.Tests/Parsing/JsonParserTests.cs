using DotBahn.Common.Parsing;
using Shouldly;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using DotBahn.Tests.Shared;
using DotBahn.Tests.Shared.Contracts;

namespace DotBahn.Common.Tests.Parsing;

public class JsonParserTests {
    private static JsonTypeInfo<SimpleDto> SimpleTypeInfo => (JsonTypeInfo<SimpleDto>)TestsJsonContext.Default.GetTypeInfo(typeof(SimpleDto))!;
    private static JsonTypeInfo<AlwaysNullDto> NullTypeInfo => (JsonTypeInfo<AlwaysNullDto>)TestsJsonContext.Default.GetTypeInfo(typeof(AlwaysNullDto))!;

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_Should_Return_Default_Instance_When_Input_Is_Empty_Or_Whitespace(string input) {
        var sut = new JsonParser<SimpleDto>(SimpleTypeInfo);

        var result = sut.Parse(input);

        result.ShouldNotBeNull();
        result.Name.ShouldBeNull();
        result.Value.ShouldBe(0);
    }

    [Fact]
    public void Parse_Should_Return_Default_Instance_When_Input_Is_Null() {
        var sut = new JsonParser<SimpleDto>(SimpleTypeInfo);

        var result = sut.Parse(null!);

        result.ShouldNotBeNull();
        result.Name.ShouldBeNull();
        result.Value.ShouldBe(0);
    }

    [Fact]
    public void Parse_Should_Return_Populated_Contract_When_Input_Is_Valid_Json() {
        var sut = new JsonParser<SimpleDto>(SimpleTypeInfo);
        const string json = """
                            {
                                "Name":"test",
                                "Value":42
                            }
                            """;

        var result = sut.Parse(json);

        result.Name.ShouldBe("test");
        result.Value.ShouldBe(42);
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("not json")]
    [InlineData("null, extra")]
    public void Parse_Should_Throw_JsonException_When_Input_Is_Invalid_Json(string input) {
        var sut = new JsonParser<SimpleDto>(SimpleTypeInfo);

        var act = () => sut.Parse(input);

        act.ShouldThrow<JsonException>();
    }

    [Fact]
    public void Parse_Should_Throw_JsonException_When_Deserialization_Returns_Null() {
        var sut = new JsonParser<AlwaysNullDto>(NullTypeInfo);
        const string json = "{}";

        var act = () => sut.Parse(json);

        act.ShouldThrow<JsonException>();
    }
}
