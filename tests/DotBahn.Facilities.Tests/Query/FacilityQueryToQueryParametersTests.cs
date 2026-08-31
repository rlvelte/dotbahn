using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Facilities.Tests.Query;

public class FacilityQueryToQueryParametersTests {
    [Fact]
    public void ToQueryParameters_WithAllPropertiesSet_ShouldConvertCorrectly() {
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

        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();

        Assert.NotNull(parameters);
        Assert.Contains("area=", qs);
    }

    [Fact]
    public void ToQueryParameters_WithMinimalProperties_ShouldConvertCorrectly() {
        var query = new FacilityQuery();

        var parameters = query.ToQueryParameters();

        Assert.NotNull(parameters);
    }

    [Fact]
    public void ToQueryParameters_WithPartialProperties_ShouldHandleNullValues() {
        var query = new FacilityQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549"
        };

        var parameters = query.ToQueryParameters();

        Assert.NotNull(parameters);
    }

    [Fact]
    public void ToQueryParameters_WithEmptyEquipmentNumbers_ShouldHandleGracefully() {
        var query = new FacilityQuery {
            EquipmentNumbers = []
        };

        var parameters = query.ToQueryParameters();

        Assert.NotNull(parameters);
    }
}
