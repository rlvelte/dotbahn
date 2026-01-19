using DotBahn.Clients.Facilities.Query;
using DotBahn.Data.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilitiesQueryTests {
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues() {
        // Arrange & Act
        var query = new FacilitiesQuery();

        // Assert
        Assert.Null(query.Type);
        Assert.Null(query.State);
        Assert.Null(query.EquipmentNumbers);
        Assert.Null(query.StationId);
    }

    [Fact]
    public void ObjectInitializer_ShouldSetAllProperties() {
        // Arrange & Act
        var query = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            EquipmentNumbers = ["10562421", "10562422"],
            StationId = "8002549"
        };

        // Assert
        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Active, query.State);
        Assert.Equal(["10562421", "10562422"], query.EquipmentNumbers);
        Assert.Equal("8002549", query.StationId);
    }

    [Fact]
    public void ObjectInitializer_WithPartialProperties_ShouldSetOnlySpecified() {
        // Arrange & Act
        var query = new FacilitiesQuery {
            Type = FacilityType.Escalator,
            StationId = "8000105"
        };

        // Assert
        Assert.Equal(FacilityType.Escalator, query.Type);
        Assert.Null(query.State);
        Assert.Null(query.EquipmentNumbers);
        Assert.Equal("8000105", query.StationId);
    }
}