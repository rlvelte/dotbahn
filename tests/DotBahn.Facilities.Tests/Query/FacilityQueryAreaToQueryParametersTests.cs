using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Facilities.Tests.Query;

public class FacilityQueryAreaToQueryParametersTests {
    [Fact]
    public void ToQueryParameters_WithArea_ShouldIncludeAreaParameter() {
        var query = new FacilityQuery {
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4
        };

        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();
        var decoded = Uri.UnescapeDataString(qs);

        Assert.Contains("area=", qs);
        Assert.Contains("8.1,50.2,8.3,50.4", decoded);
    }

    [Fact]
    public void ToQueryParameters_WithoutArea_ShouldOmitAreaParameter() {
        var query = new FacilityQuery();

        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();

        Assert.DoesNotContain("area", qs);
    }

    [Fact]
    public void ToQueryParameters_WithPartialArea_ShouldOmitAreaParameter() {
        var query = new FacilityQuery {
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2
            // LongitudeEast and LatitudeNorth are null
        };

        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();

        Assert.DoesNotContain("area", qs);
    }

    [Fact]
    public void ToQueryParameters_WithAreaOnly_ShouldContainAreaWithAllCoordinates() {
        var query = new FacilityQuery {
            LongitudeWest = -122.41,
            LatitudeSouth = 37.77,
            LongitudeEast = -122.01,
            LatitudeNorth = 37.79
        };

        var parameters = query.ToQueryParameters();
        var qs = parameters.ToQueryString();
        var decoded = Uri.UnescapeDataString(qs);

        Assert.Contains("area=", qs);
        Assert.Equal("-122.41,37.77,-122.01,37.79", decoded["area=".Length..]);
    }

    public static TheoryData<string, double?, double?, double?, double?, bool> ToQueryParametersAreaAllFourRequiredCases => new()
    {
        { "BL—all_set", 8.0, 50.0, 9.0, 51.0, true },
        { "C1—lngw_null", null, 50.0, 9.0, 51.0, false },
        { "C2—lats_null", 8.0, null, 9.0, 51.0, false },
        { "C3—lnge_null", 8.0, 50.0, null, 51.0, false },
        { "C4—latn_null", 8.0, 50.0, 9.0, null, false },
    };

    [Theory]
    [MemberData(nameof(ToQueryParametersAreaAllFourRequiredCases))]
    public void ToQueryParametersAreaAllFourRequired(string _, double? lngWest, double? latSouth, double? lngEast, double? latNorth, bool expectedHasArea) {
        var query = new FacilityQuery { LongitudeWest = lngWest, LatitudeSouth = latSouth, LongitudeEast = lngEast, LatitudeNorth = latNorth };
        var qs = query.ToQueryParameters().ToQueryString();
        if (expectedHasArea)
            Assert.Contains("area=", qs);
        else
            Assert.DoesNotContain("area=", qs);
    }

    [Fact]
    public void ToQueryParameters_WithAreaAndOtherFilters_ShouldCombineAllParameters() {
        var query = new FacilityQuery {
            Type = FacilityType.Elevator,
            State = FacilityState.Inactive,
            LongitudeWest = 8.1,
            LatitudeSouth = 50.2,
            LongitudeEast = 8.3,
            LatitudeNorth = 50.4,
            StationId = "8000105"
        };

        var qs = query.ToQueryParameters().ToQueryString();

        Assert.Contains("area=", qs);
        Assert.Contains("type=", qs);
        Assert.Contains("state=", qs);
        Assert.Contains("stationnumber=8000105", qs);
    }
}
