using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryTests {
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues() {
        // Arrange & Act
        var query = new FacilityQuery();

        // Assert
        Assert.Null(query.Type);
        Assert.Null(query.State);
        Assert.Empty(query.EquipmentNumbers);
        Assert.Null(query.StationId);
        Assert.Null(query.LongitudeWest);
        Assert.Null(query.LatitudeSouth);
        Assert.Null(query.LongitudeEast);
        Assert.Null(query.LatitudeNorth);
    }

    [Fact]
    public void ObjectInitializer_ShouldSetAllProperties() {
        // Arrange & Act
        var query = new FacilityQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            EquipmentNumbers = ["10562421", "10562422"],
            StationId = "8002549",
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4
        };

        // Assert
        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Active, query.State);
        Assert.Equal(["10562421", "10562422"], query.EquipmentNumbers);
        Assert.Equal("8002549", query.StationId);
        Assert.Equal(8.1, query.LongitudeWest);
        Assert.Equal(50.2, query.LatitudeSouth);
        Assert.Equal(8.3, query.LongitudeEast);
        Assert.Equal(50.4, query.LatitudeNorth);
    }

    [Fact]
    public void ObjectInitializer_WithPartialProperties_ShouldSetOnlySpecified() {
        // Arrange & Act
        var query = new FacilityQuery {
            Type = FacilityType.Escalator,
            StationId = "8000105"
        };

        // Assert
        Assert.Equal(FacilityType.Escalator, query.Type);
        Assert.Null(query.State);
        Assert.Empty(query.EquipmentNumbers);
        Assert.Equal("8000105", query.StationId);
        Assert.Null(query.LongitudeWest);
        Assert.Null(query.LatitudeSouth);
        Assert.Null(query.LongitudeEast);
        Assert.Null(query.LatitudeNorth);
    }
}
