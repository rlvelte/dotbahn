using DotBahn.Clients.Facilities.Query;
using DotBahn.Data.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilitiesQueryRecordBehaviorTests {
    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual() {
        // Arrange
        var query1 = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            StationId = "8002549"
        };

        var query2 = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            StationId = "8002549"
        };

        // Act & Assert
        Assert.Equal(query1, query2);
        Assert.True(query1 == query2);
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual() {
        // Arrange
        var query1 = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549"
        };

        var query2 = new FacilitiesQuery {
            Type = FacilityType.Escalator,
            StationId = "8002549"
        };

        // Act & Assert
        Assert.NotEqual(query1, query2);
        Assert.False(query1 == query2);
    }

    [Fact]
    public void With_ShouldCreateNewInstanceWithModifiedProperty() {
        // Arrange
        var original = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            StationId = "8002549"
        };

        // Act
        var modified = original with { Type = FacilityType.Escalator };

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(FacilityType.Elevator, original.Type);
        Assert.Equal(FacilityType.Escalator, modified.Type);
        Assert.Equal(original.State, modified.State);
        Assert.Equal(original.StationId, modified.StationId);
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldBeEqual() {
        // Arrange
        var query1 = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549"
        };

        var query2 = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549"
        };

        // Act & Assert
        Assert.Equal(query1.GetHashCode(), query2.GetHashCode());
    }
}