using DotBahn.Facilities;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryStationIdTests {
    [Theory]
    [InlineData("8002549")]
    [InlineData("8000105")]
    [InlineData("8011160")]
    public void StationId_ShouldSetAndGetValue(string stationId) {
        var query = new FacilityQuery {
            StationId = stationId
        };

        Assert.Equal(stationId, query.StationId);
    }

    [Fact]
    public void StationId_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { StationId = "8002549" };

        query.StationId = null;

        Assert.Null(query.StationId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void StationId_WithEmptyOrWhitespace_ShouldSetValue(string stationId) {
        var query = new FacilityQuery {
            StationId = stationId
        };

        Assert.Equal(stationId, query.StationId);
    }

    [Theory]
    [InlineData(8002549)]
    [InlineData(8000105)]
    [InlineData(8011160)]
    public void AtStation_ShouldConvertIntToStringAndReturnQuery(int stationId) {
        var query = new FacilityQuery();

        var result = query.AtStation(stationId);

        Assert.Same(query, result);
        Assert.Equal(stationId.ToString(), query.StationId);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(-1, "-1")]
    [InlineData(999999999, "999999999")]
    public void AtStation_WithEdgeCaseValues_ShouldConvertCorrectly(int stationId, string expected) {
        var query = new FacilityQuery();

        query.AtStation(stationId);

        Assert.Equal(expected, query.StationId);
    }
}
