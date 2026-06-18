using DotBahn.Facilities.Enumerations;
using DotBahn.Facilities.Json;
using DotBahn.Shared.Enumerations;

namespace DotBahn.Tests.Facilities.Enumerations;

public class EnumMapperTests {
    [Theory]
    [InlineData("ACTIVE", FacilityState.Active)]
    [InlineData("INACTIVE", FacilityState.Inactive)]
    [InlineData("UNKNOWN", FacilityState.Unknown)]
    public void Parse_WithExactCaseValue_ReturnsCorrectEnum(string value, FacilityState expected) {
        var result = EnumMapper.Parse(value, FacilityState.Unknown, FacilitiesJsonContext.Default.FacilityState);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_WithNull_ReturnsDefault() {
        var result = EnumMapper.Parse(null, FacilityState.Unknown, FacilitiesJsonContext.Default.FacilityState);

        Assert.Equal(FacilityState.Unknown, result);
    }

    [Fact]
    public void Parse_WithEmptyString_ReturnsDefault() {
        var result = EnumMapper.Parse(string.Empty, FacilityState.Unknown, FacilitiesJsonContext.Default.FacilityState);

        Assert.Equal(FacilityState.Unknown, result);
    }

    [Fact]
    public void Parse_WithUnknownValue_ReturnsDefault() {
        var result = EnumMapper.Parse("NONEXISTENT", FacilityState.Unknown, FacilitiesJsonContext.Default.FacilityState);

        Assert.Equal(FacilityState.Unknown, result);
    }

    [Fact]
    public void Format_WithValidEnum_ReturnsCorrectString() {
        var result = EnumMapper.Format(FacilityState.Active, FacilitiesJsonContext.Default.FacilityState);

        Assert.Equal("ACTIVE", result);
    }

    [Fact]
    public void Format_WithNull_ReturnsNull() {
        FacilityState? nullState = null;
        var result = EnumMapper.Format(nullState, FacilitiesJsonContext.Default.FacilityState);

        Assert.Null(result);
    }

    [Fact]
    public void Format_WithInactive_ReturnsInactiveString() {
        var result = EnumMapper.Format(FacilityState.Inactive, FacilitiesJsonContext.Default.FacilityState);

        Assert.Equal("INACTIVE", result);
    }

    [Theory]
    [InlineData(FacilityType.Elevator, "ELEVATOR")]
    [InlineData(FacilityType.Escalator, "ESCALATOR")]
    public void Format_FacilityType_ReturnsCorrectString(FacilityType value, string expected) {
        var result = EnumMapper.Format(value, FacilitiesJsonContext.Default.FacilityType);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ELEVATOR", FacilityType.Elevator)]
    [InlineData("ESCALATOR", FacilityType.Escalator)]
    public void Parse_FacilityType_ReturnsCorrectEnum(string value, FacilityType expected) {
        var result = EnumMapper.Parse(value, FacilityType.Unknown, FacilitiesJsonContext.Default.FacilityType);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_FacilityType_UnknownValue_ReturnsDefault() {
        var result = EnumMapper.Parse("BOGEY", FacilityType.Unknown, FacilitiesJsonContext.Default.FacilityType);

        Assert.Equal(FacilityType.Unknown, result);
    }
}
