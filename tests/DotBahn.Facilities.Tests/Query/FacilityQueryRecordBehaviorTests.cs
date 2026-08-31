using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Facilities.Tests.Query;

public class FacilityQueryRecordBehaviorTests {
    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual() {
        var query1 = new FacilityQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            StationId = "8002549",
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4
        };

        var query2 = new FacilityQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            StationId = "8002549",
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4
        };

        Assert.Equal(query1, query2);
        Assert.True(query1 == query2);
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual() {
        var query1 = new FacilityQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549",
            LongitudeWest = 8.1
        };

        var query2 = new FacilityQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549",
            LongitudeWest = 9.9
        };

        Assert.NotEqual(query1, query2);
        Assert.False(query1 == query2);
    }

    [Fact]
    public void With_ShouldCreateNewInstanceWithModifiedProperty() {
        var original = new FacilityQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Active,
            StationId = "8002549",
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4
        };

        var modified = original with { Type = FacilityType.Escalator };

        Assert.NotSame(original, modified);
        Assert.Equal(FacilityType.Elevator, original.Type);
        Assert.Equal(FacilityType.Escalator, modified.Type);
        Assert.Equal(original.State, modified.State);
        Assert.Equal(original.StationId, modified.StationId);
        Assert.Equal(original.LongitudeWest, modified.LongitudeWest);
        Assert.Equal(original.LatitudeSouth, modified.LatitudeSouth);
        Assert.Equal(original.LongitudeEast, modified.LongitudeEast);
        Assert.Equal(original.LatitudeNorth, modified.LatitudeNorth);
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldBeEqual() {
        var query1 = new FacilityQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549",
            LongitudeWest = 8.1
        };

        var query2 = new FacilityQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549",
            LongitudeWest = 8.1
        };

        Assert.Equal(query1.GetHashCode(), query2.GetHashCode());
    }
}
