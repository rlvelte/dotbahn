using DotBahn.Facilities;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryAreaTests {
    [Fact]
    public void Area_Default_AllCoordinatesShouldBeNull() {
        var query = new FacilityQuery();

        Assert.Null(query.LongitudeWest);
        Assert.Null(query.LatitudeSouth);
        Assert.Null(query.LongitudeEast);
        Assert.Null(query.LatitudeNorth);
    }

    [Fact]
    public void Area_WithFourCoordinates_ShouldSetValues() {
        var query = new FacilityQuery {
            LongitudeWest = 8.6821,
            LatitudeSouth = 50.1109,
            LongitudeEast = 8.6822,
            LatitudeNorth = 50.1110
        };

        Assert.Equal(8.6821, query.LongitudeWest);
        Assert.Equal(50.1109, query.LatitudeSouth);
        Assert.Equal(8.6822, query.LongitudeEast);
        Assert.Equal(50.1110, query.LatitudeNorth);
    }

    [Fact]
    public void Area_NegativeCoordinates_ShouldSetValues() {
        var query = new FacilityQuery {
            LongitudeWest = -0.1276,
            LatitudeSouth = 51.5074,
            LongitudeEast = 0.0,
            LatitudeNorth = 51.5075
        };

        Assert.Equal(-0.1276, query.LongitudeWest);
        Assert.Equal(51.5074, query.LatitudeSouth);
    }

    [Fact]
    public void Area_PartialCoordinates_ShouldAllowPartialSetting() {
        var query = new FacilityQuery {
            LongitudeWest = 8.6821,
            LatitudeSouth = 50.1109
        };

        Assert.Equal(8.6821, query.LongitudeWest);
        Assert.Equal(50.1109, query.LatitudeSouth);
        Assert.Null(query.LongitudeEast);
        Assert.Null(query.LatitudeNorth);
    }

    [Fact]
    public void LongitudeWest_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { LongitudeWest = 8.6821 };

        query.LongitudeWest = null;

        Assert.Null(query.LongitudeWest);
    }

    [Fact]
    public void LatitudeSouth_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { LatitudeSouth = 50.1109 };

        query.LatitudeSouth = null;

        Assert.Null(query.LatitudeSouth);
    }

    [Fact]
    public void LongitudeEast_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { LongitudeEast = 8.6822 };

        query.LongitudeEast = null;

        Assert.Null(query.LongitudeEast);
    }

    [Fact]
    public void LatitudeNorth_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { LatitudeNorth = 50.1110 };

        query.LatitudeNorth = null;

        Assert.Null(query.LatitudeNorth);
    }
}
