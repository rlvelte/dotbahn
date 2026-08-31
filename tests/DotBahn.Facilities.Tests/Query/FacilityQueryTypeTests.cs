using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Facilities.Tests.Query;

public class FacilityQueryTypeTests {
    [Theory]
    [InlineData(FacilityType.Elevator)]
    [InlineData(FacilityType.Escalator)]
    public void Type_ShouldSetAndGetValue(FacilityType type) {
        var query = new FacilityQuery {
            Type = type
        };

        Assert.Equal(type, query.Type);
    }

    [Fact]
    public void Type_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { Type = FacilityType.Elevator };

        query.Type = null;

        Assert.Null(query.Type);
    }

    [Fact]
    public void WithType_ShouldSetTypeAndReturnQuery() {
        var query = new FacilityQuery();

        var result = query.WithType(FacilityType.Elevator);

        Assert.Same(query, result);
        Assert.Equal(FacilityType.Elevator, query.Type);
    }

    [Theory]
    [InlineData(FacilityType.Elevator)]
    [InlineData(FacilityType.Escalator)]
    public void WithType_WithDifferentTypes_ShouldSetCorrectType(FacilityType type) {
        var query = new FacilityQuery();

        query.WithType(type);

        Assert.Equal(type, query.Type);
    }
}
