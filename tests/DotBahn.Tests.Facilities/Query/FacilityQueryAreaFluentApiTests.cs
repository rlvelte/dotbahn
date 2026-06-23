using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryAreaFluentApiTests {
    [Fact]
    public void WithArea_FourCoordinates_ShouldSetAllProperties() {
        // Arrange & Act
        var query = new FacilityQuery()
            .WithArea(8.6821, 50.1109, 8.6822, 50.1110);

        // Assert
        Assert.Equal(8.6821, query.LongitudeWest);
        Assert.Equal(50.1109, query.LatitudeSouth);
        Assert.Equal(8.6822, query.LongitudeEast);
        Assert.Equal(50.1110, query.LatitudeNorth);
    }

    [Fact]
    public void WithArea_CalledMultipleTimes_ShouldOverwritePreviousValues() {
        // Arrange & Act
        var query = new FacilityQuery()
            .WithArea(8.6821, 50.1109, 8.6822, 50.1110)
            .WithArea(9.0, 51.0, 10.0, 52.0);

        // Assert
        Assert.Equal(9.0, query.LongitudeWest);
        Assert.Equal(51.0, query.LatitudeSouth);
        Assert.Equal(10.0, query.LongitudeEast);
        Assert.Equal(52.0, query.LatitudeNorth);
    }

    [Fact]
    public void WithArea_ReturnsSameInstance() {
        // Arrange
        var query = new FacilityQuery();

        // Act
        var result = query.WithArea(8.6821, 50.1109, 8.6822, 50.1110);

        // Assert
        Assert.Same(query, result);
    }

    [Fact]
    public void CompleteChaining_WithArea_ShouldSetAllProperties() {
        // Arrange & Act
        var query = new FacilityQuery()
            .WithType(FacilityType.Elevator)
            .WithState(FacilityState.Inactive)
            .WithEquipmentNumbers("10562421")
            .AtStation(8002549)
            .WithArea(8.1, 50.2, 8.3, 50.4);

        // Assert
        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Inactive, query.State);
        Assert.Equal(["10562421"], query.EquipmentNumbers);
        Assert.Equal("8002549", query.StationId);
        Assert.Equal(8.1, query.LongitudeWest);
        Assert.Equal(50.2, query.LatitudeSouth);
        Assert.Equal(8.3, query.LongitudeEast);
        Assert.Equal(50.4, query.LatitudeNorth);
    }

    [Fact]
    public void FluentApi_PartialChaining_WithoutArea_ShouldLeaveAreaPropertiesNull() {
        // Arrange & Act
        var query = new FacilityQuery()
            .WithType(FacilityType.Escalator)
            .AtStation(8000105);

        // Assert
        Assert.Equal(FacilityType.Escalator, query.Type);
        Assert.Equal("8000105", query.StationId);
        Assert.Null(query.LongitudeWest);
        Assert.Null(query.LatitudeSouth);
        Assert.Null(query.LongitudeEast);
        Assert.Null(query.LatitudeNorth);
    }

    [Fact]
    public void FluentApi_MixedWithObjectInitializer_ShouldCombineBothApproaches() {
        // Arrange & Act
        var query = new FacilityQuery {
            Type = FacilityType.Elevator
        }.WithState(FacilityState.Active)
         .WithArea(8.1, 50.2, 8.3, 50.4);

        // Assert
        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Active, query.State);
        Assert.Equal(8.1, query.LongitudeWest);
        Assert.Equal(50.2, query.LatitudeSouth);
        Assert.Equal(8.3, query.LongitudeEast);
        Assert.Equal(50.4, query.LatitudeNorth);
    }

    [Fact]
    public void ChainingWithAreaAndOtherMethods_ReturnsSameInstance() {
        // Arrange
        var query = new FacilityQuery();

        // Act
        var result = query
            .WithType(FacilityType.Escalator)
            .WithArea(8.6821, 50.1109, 8.6822, 50.1110)
            .AtStation(8000105);

        // Assert
        Assert.Same(query, result);
    }
}
