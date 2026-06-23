using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryAreaToQueryParametersTests {
    [Fact]
    public void ToQueryParameters_WithArea_ShouldIncludeAreaParameter() {
        // Arrange
        var query = new FacilityQuery {
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4
        };

        // Act
        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();
        var decoded = Uri.UnescapeDataString(qs);

        // Assert
        Assert.Contains("area=", qs);
        Assert.Contains("8.1,50.2,8.3,50.4", decoded);
    }

    [Fact]
    public void ToQueryParameters_WithoutArea_ShouldOmitAreaParameter() {
        // Arrange
        var query = new FacilityQuery();

        // Act
        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();

        // Assert
        Assert.DoesNotContain("area", qs);
    }

    [Fact]
    public void ToQueryParameters_WithPartialArea_ShouldOmitAreaParameter() {
        // Arrange
        var query = new FacilityQuery {
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2
            // LongitudeEast and LatitudeNorth are null
        };

        // Act
        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();

        // Assert
        Assert.DoesNotContain("area", qs);
    }

    [Fact]
    public void ToQueryParameters_WithAreaOnly_ShouldContainAreaWithAllCoordinates() {
        // Arrange
        var query = new FacilityQuery {
            LongitudeWest = -122.41,
            LatitudeSouth = 37.77,
            LongitudeEast = -122.01,
            LatitudeNorth = 37.79
        };

        // Act
        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();
        var decoded = Uri.UnescapeDataString(qs);

        // Assert
        Assert.Contains("area=", qs);
        Assert.Equal("-122.41,37.77,-122.01,37.79", decoded["area=".Length..]);
    }

    [Fact]
    public void ToQueryParameters_WithAreaAndOtherFilters_ShouldCombineAllParameters() {
        // Arrange
        var query = new FacilityQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Inactive,
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4,
            StationId = "8000105"
        };

        // Act
        var qs = query.ToQueryParameters().ToQueryString();

        // Assert
        Assert.Contains("area=", qs);
        Assert.Contains("type=", qs);
        Assert.Contains("state=", qs);
        Assert.Contains("stationnumber=8000105", qs);
    }
}
