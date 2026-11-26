using DotBahn.Common.Utilities;
using Shouldly;

namespace DotBahn.Tests.Common.Utilities;

public class QueryParametersTests {
    [Fact]
    public void Create_Should_Return_Empty_Instance_When_Called() {
        var sut = QueryParameters.Create();

        sut.Any().ShouldBeFalse();
        sut.ToQueryString().ShouldBeEmpty();
    }

    [Fact]
    public void Add_Should_Store_Value_When_Value_Is_NonWhitespace() {
        var sut = QueryParameters.Create().Add("key", "value");

        sut.Any().ShouldBeTrue();
        sut.ToQueryString().ShouldBe("key=value");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_Should_Ignore_Value_When_Value_Is_Null_Or_Whitespace(string? value) {
        var sut = QueryParameters.Create().Add("key", value);

        sut.Any().ShouldBeFalse();
    }

    [Fact]
    public void Add_Should_Overwrite_Existing_Key_When_Duplicate_Added() {
        var sut = QueryParameters.Create().Add("key", "first").Add("key", "second");

        sut.ToQueryString().ShouldBe("key=second");
    }

    [Fact]
    public void Add_Collection_Should_Join_Values_With_Comma_When_Values_Provided() {
        var sut = QueryParameters.Create().Add("ids", [1, 2, 3]);

        sut.ToQueryString().ShouldBe("ids=1%2C2%2C3");
    }

    [Fact]
    public void Add_Collection_Should_Ignore_When_Values_Are_Null() {
        var sut = QueryParameters.Create().Add<int>("ids", null);

        sut.Any().ShouldBeFalse();
    }

    [Fact]
    public void Add_Collection_Should_Ignore_When_Values_Are_Empty() {
        var sut = QueryParameters.Create().Add("ids", Array.Empty<int>());

        sut.Any().ShouldBeFalse();
    }

    [Fact]
    public void ToQueryString_Should_UrlEncode_Keys_And_Values() {
        var sut = QueryParameters.Create().Add("key name", "value&=?");

        sut.ToQueryString().ShouldBe("key%20name=value%26%3D%3F");
    }

    [Fact]
    public void Add_Should_Return_Same_Instance_For_Fluent_Chaining() {
        var sut = QueryParameters.Create();

        sut.Add("a", "1").ShouldBeSameAs(sut);
    }
}
