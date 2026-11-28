using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;
using Shouldly;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryTests {
    [Fact]
    public void Constructor_Should_UseDocumentedDefaults_When_Called() {
        var sut = new FacilityQuery();

        sut.Type.ShouldBeNull();
        sut.State.ShouldBeNull();
        sut.EquipmentNumbers.ShouldBeEmpty();
        sut.StationId.ShouldBeNull();
        sut.LongitudeWest.ShouldBeNull();
        sut.LatitudeSouth.ShouldBeNull();
        sut.LongitudeEast.ShouldBeNull();
        sut.LatitudeNorth.ShouldBeNull();
    }

    [Theory]
    [InlineData(FacilityType.Elevator)]
    [InlineData(FacilityType.Escalator)]
    [InlineData(FacilityType.Unknown)]
    public void WithType_Should_SetType_When_ValueProvided(FacilityType type) {
        var sut = new FacilityQuery();

        var result = sut.WithType(type);

        result.ShouldBeSameAs(sut);
        sut.Type.ShouldBe(type);
    }

    [Theory]
    [InlineData(FacilityState.Active)]
    [InlineData(FacilityState.Inactive)]
    [InlineData(FacilityState.Unknown)]
    public void WithState_Should_SetState_When_ValueProvided(FacilityState state) {
        var sut = new FacilityQuery();

        var result = sut.WithState(state);

        result.ShouldBeSameAs(sut);
        sut.State.ShouldBe(state);
    }

    [Theory]
    [InlineData("10562421")]
    [InlineData("10562421", "10562422")]
    [InlineData]
    public void WithEquipmentNumbers_Should_SetNumbers_When_ValuesProvided(params string[] numbers) {
        var sut = new FacilityQuery();

        var result = sut.WithEquipmentNumbers(numbers);

        result.ShouldBeSameAs(sut);
        sut.EquipmentNumbers.ShouldBe(numbers);
    }

    [Fact]
    public void WithEquipmentNumbers_Should_SetNull_When_NullProvided() {
        var sut = new FacilityQuery();

        sut.WithEquipmentNumbers(null!);

        sut.EquipmentNumbers.ShouldBeNull();
    }

    [Theory]
    [InlineData(-1, "-1")]
    [InlineData(0, "0")]
    [InlineData(8002549, "8002549")]
    [InlineData(int.MaxValue, "2147483647")]
    [InlineData(int.MinValue, "-2147483648")]
    public void AtStation_Should_SetStationId_When_ValueProvided(int stationId, string expected) {
        var sut = new FacilityQuery();

        var result = sut.AtStation(stationId);

        result.ShouldBeSameAs(sut);
        sut.StationId.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0d, 0d, 0d, 0d)]
    [InlineData(-180d, -90d, 180d, 90d)]
    [InlineData(8.1d, 50.2d, 8.3d, 50.4d)]
    [InlineData(double.MaxValue, double.MinValue, double.Epsilon, double.NaN)]
    public void WithArea_Should_SetAllBounds_When_ValuesProvided(double west, double south, double east, double north) {
        var sut = new FacilityQuery();

        var result = sut.WithArea(west, south, east, north);

        result.ShouldBeSameAs(sut);
        sut.LongitudeWest.ShouldBe(west);
        sut.LatitudeSouth.ShouldBe(south);
        sut.LongitudeEast.ShouldBe(east);
        sut.LatitudeNorth.ShouldBe(north);
    }
}
