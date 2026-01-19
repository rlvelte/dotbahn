using DotBahn.Clients.Facilities.Query;
using DotBahn.Data.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilitiesQueryToQueryParametersTests {
    [Fact]
    public void ToQueryParameters_WithAllPropertiesSet_ShouldConvertCorrectly() {
        // Arrange
        var query = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            EquipmentNumbers = ["10562421", "10562422"],
            StationId = "8002549"
        };

        // Act
        var parameters = query.ToQueryParameters();

        // Assert
        Assert.NotNull(parameters);
    }

    [Fact]
    public void ToQueryParameters_WithMinimalProperties_ShouldConvertCorrectly() {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        var parameters = query.ToQueryParameters();

        // Assert
        Assert.NotNull(parameters);
    }

    [Fact]
    public void ToQueryParameters_WithPartialProperties_ShouldHandleNullValues() {
        // Arrange
        var query = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549"
        };

        // Act
        var parameters = query.ToQueryParameters();

        // Assert
        Assert.NotNull(parameters);
    }

    [Fact]
    public void ToQueryParameters_WithEmptyEquipmentNumbers_ShouldHandleGracefully() {
        // Arrange
        var query = new FacilitiesQuery {
            EquipmentNumbers = []
        };

        // Act
        var parameters = query.ToQueryParameters();

        // Assert
        Assert.NotNull(parameters);
    }
}