using DotBahn.Clients.Facilities.Query;

namespace DotBahn.Tests.Facilities.Query;

public class FacilitiesQueryStationIdTests {
    [Theory]
    [InlineData("8002549")]
    [InlineData("8000105")]
    [InlineData("8011160")]
    public void StationId_ShouldSetAndGetValue(string stationId) {
        // Arrange & Act
        var query = new FacilitiesQuery {
            StationId = stationId
        };

        // Assert
        Assert.Equal(stationId, query.StationId);
    }

    [Fact]
    public void StationId_WithNull_ShouldSetNull() {
        // Arrange
        var query = new FacilitiesQuery { StationId = "8002549" };

        // Act
        query.StationId = null;

        // Assert
        Assert.Null(query.StationId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void StationId_WithEmptyOrWhitespace_ShouldSetValue(string stationId) {
        // Arrange & Act
        var query = new FacilitiesQuery {
            StationId = stationId
        };

        // Assert
        Assert.Equal(stationId, query.StationId);
    }

    [Theory]
    [InlineData(8002549)]
    [InlineData(8000105)]
    [InlineData(8011160)]
    public void AtStation_ShouldConvertIntToStringAndReturnQuery(int stationId) {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        var result = query.AtStation(stationId);

        // Assert
        Assert.Same(query, result);
        Assert.Equal(stationId.ToString(), query.StationId);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(-1, "-1")]
    [InlineData(999999999, "999999999")]
    public void AtStation_WithEdgeCaseValues_ShouldConvertCorrectly(int stationId, string expected) {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        query.AtStation(stationId);

        // Assert
        Assert.Equal(expected, query.StationId);
    }
}