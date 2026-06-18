using DotBahn.Facilities;
using DotBahn.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryStateTests {
    [Theory]
    [InlineData(FacilityState.Active)]
    [InlineData(FacilityState.Inactive)]
    [InlineData(FacilityState.Unknown)]
    public void State_ShouldSetAndGetValue(FacilityState state) {
        // Arrange & Act
        var query = new FacilityQuery {
            State = state
        };

        // Assert
        Assert.Equal(state, query.State);
    }

    [Fact]
    public void State_WithNull_ShouldSetNull() {
        // Arrange
        var query = new FacilityQuery { State = FacilityState.Active };

        // Act
        query.State = null;

        // Assert
        Assert.Null(query.State);
    }

    [Fact]
    public void WithState_ShouldSetStateAndReturnQuery() {
        // Arrange
        var query = new FacilityQuery();

        // Act
        var result = query.WithState(FacilityState.Active);

        // Assert
        Assert.Same(query, result);
        Assert.Equal(FacilityState.Active, query.State);
    }

    [Theory]
    [InlineData(FacilityState.Active)]
    [InlineData(FacilityState.Inactive)]
    [InlineData(FacilityState.Unknown)]
    public void WithState_WithDifferentStates_ShouldSetCorrectState(FacilityState state) {
        // Arrange
        var query = new FacilityQuery();

        // Act
        query.WithState(state);

        // Assert
        Assert.Equal(state, query.State);
    }
}
