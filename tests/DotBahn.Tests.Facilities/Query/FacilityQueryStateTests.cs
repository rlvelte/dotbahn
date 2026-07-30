using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryStateTests {
    [Theory]
    [InlineData(FacilityState.Active)]
    [InlineData(FacilityState.Inactive)]
    [InlineData(FacilityState.Unknown)]
    public void State_ShouldSetAndGetValue(FacilityState state) {
        var query = new FacilityQuery {
            State = state
        };

        Assert.Equal(state, query.State);
    }

    [Fact]
    public void State_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { State = FacilityState.Active };

        query.State = null;

        Assert.Null(query.State);
    }

    [Fact]
    public void WithState_ShouldSetStateAndReturnQuery() {
        var query = new FacilityQuery();

        var result = query.WithState(FacilityState.Active);

        Assert.Same(query, result);
        Assert.Equal(FacilityState.Active, query.State);
    }

    [Theory]
    [InlineData(FacilityState.Active)]
    [InlineData(FacilityState.Inactive)]
    [InlineData(FacilityState.Unknown)]
    public void WithState_WithDifferentStates_ShouldSetCorrectState(FacilityState state) {
        var query = new FacilityQuery();

        query.WithState(state);

        Assert.Equal(state, query.State);
    }
}
