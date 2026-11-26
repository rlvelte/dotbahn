using DotBahn.Common.Utilities;
using Shouldly;
using System.Text.Json.Serialization.Metadata;
using DotBahn.Tests.Shared;
using DotBahn.Tests.Shared.Contracts;

namespace DotBahn.Tests.Common.Utilities;

public class EnumUtilTests {
    private static JsonTypeInfo<TestEnum> TypeInfo => (JsonTypeInfo<TestEnum>)TestsJsonContext.Default.GetTypeInfo(typeof(TestEnum))!;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Parse_Should_Return_Default_Value_When_Value_Is_Null_Or_Empty(string? value) {
        var result = EnumUtil.Parse(value!, TestEnum.Unknown, TypeInfo);

        result.ShouldBe(TestEnum.Unknown);
    }

    [Fact]
    public void Parse_Should_Return_Enum_Value_When_Value_Is_Valid_Name() {
        var result = EnumUtil.Parse("Active", TestEnum.Unknown, TypeInfo);

        result.ShouldBe(TestEnum.Active);
    }

    [Fact]
    public void Parse_Should_Return_Default_Value_When_Value_Is_Invalid_Name() {
        var result = EnumUtil.Parse("NonExistent", TestEnum.Unknown, TypeInfo);

        result.ShouldBe(TestEnum.Unknown);
    }

    [Fact]
    public void Parse_Should_Be_Case_Sensitive_When_Value_Has_Different_Casing() {
        var result = EnumUtil.Parse("active", TestEnum.Unknown, TypeInfo);

        result.ShouldBe(TestEnum.Unknown);
    }

    [Fact]
    public void Format_Should_Return_Null_When_Value_Is_Null() {
        var result = EnumUtil.Format(null, TypeInfo);

        result.ShouldBeNull();
    }

    [Theory]
    [InlineData(TestEnum.Active, "Active")]
    [InlineData(TestEnum.Inactive, "Inactive")]
    [InlineData(TestEnum.Unknown, "Unknown")]
    public void Format_Should_Return_String_Name_When_Value_Is_Provided(TestEnum value, string expected) {
        var result = EnumUtil.Format(value, TypeInfo);

        result.ShouldBe(expected);
    }
}
