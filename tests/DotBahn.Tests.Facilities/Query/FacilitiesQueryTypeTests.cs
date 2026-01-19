using DotBahn.Clients.Facilities.Query;
using DotBahn.Data.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilitiesQueryTypeTests {
    [Theory]
    [InlineData(FacilityType.Elevator)]
    [InlineData(FacilityType.Escalator)]
    public void Type_ShouldSetAndGetValue(FacilityType type) {
        // Arrange & Act
        var query = new FacilitiesQuery {
            Type = type
        };

        // Assert
        Assert.Equal(type, query.Type);
    }

    [Fact]
    public void Type_WithNull_ShouldSetNull() {
        // Arrange
        var query = new FacilitiesQuery { Type = FacilityType.Elevator };

        // Act
        query.Type = null;

        // Assert
        Assert.Null(query.Type);
    }

    [Fact]
    public void WithType_ShouldSetTypeAndReturnQuery() {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        var result = query.WithType(FacilityType.Elevator);

        // Assert
        Assert.Same(query, result);
        Assert.Equal(FacilityType.Elevator, query.Type);
    }

    [Theory]
    [InlineData(FacilityType.Elevator)]
    [InlineData(FacilityType.Escalator)]
    public void WithType_WithDifferentTypes_ShouldSetCorrectType(FacilityType type) {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        query.WithType(type);

        // Assert
        Assert.Equal(type, query.Type);
    }
}