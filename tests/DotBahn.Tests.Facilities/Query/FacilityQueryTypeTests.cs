using DotBahn.Facilities;
using DotBahn.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryTypeTests {
    [Theory]
    [InlineData(FacilityType.Elevator)]
    [InlineData(FacilityType.Escalator)]
    public void Type_ShouldSetAndGetValue(FacilityType type) {
        // Arrange & Act
        var query = new FacilityQuery {
            Type = type
        };

        // Assert
        Assert.Equal(type, query.Type);
    }

    [Fact]
    public void Type_WithNull_ShouldSetNull() {
        // Arrange
        var query = new FacilityQuery { Type = FacilityType.Elevator };

        // Act
        query.Type = null;

        // Assert
        Assert.Null(query.Type);
    }

    [Fact]
    public void WithType_ShouldSetTypeAndReturnQuery() {
        // Arrange
        var query = new FacilityQuery();

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
        var query = new FacilityQuery();

        // Act
        query.WithType(type);

        // Assert
        Assert.Equal(type, query.Type);
    }
}
