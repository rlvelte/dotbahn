using DotBahn.Stations;
using DotBahn.Stations.Models.Enumerations;

namespace DotBahn.Tests.Stations.Query;

public class StationQueryOtherPropertiesTests {
    [Theory]
    [InlineData(FederalState.Hamburg)]
    [InlineData(FederalState.Bavaria)]
    [InlineData(FederalState.Berlin)]
    public void State_ShouldSetAndGetValue(FederalState state) {
        var query = new StationQuery {
            State = state
        };

        Assert.Equal(state, query.State);
    }

    [Theory]
    [InlineData("8002549")]
    [InlineData("8000105")]
    public void Eva_ShouldSetAndGetValue(string eva) {
        var query = new StationQuery {
            Eva = eva
        };

        Assert.Equal(eva, query.Eva);
    }

    [Theory]
    [InlineData("AH")]
    [InlineData("BL")]
    public void Ril_ShouldSetAndGetValue(string ril) {
        var query = new StationQuery {
            Ril = ril
        };

        Assert.Equal(ril, query.Ril);
    }

    [Theory]
    [InlineData(LogicalOperator.And)]
    [InlineData(LogicalOperator.Or)]
    public void Operator_ShouldSetAndGetValue(LogicalOperator op) {
        var query = new StationQuery {
            Operator = op
        };

        Assert.Equal(op, query.Operator);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100)]
    public void Offset_ShouldSetAndGetValue(int offset) {
        var query = new StationQuery {
            Offset = offset
        };

        Assert.Equal(offset, query.Offset);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(10000)]
    public void Limit_ShouldSetAndGetValue(int limit) {
        var query = new StationQuery {
            Limit = limit
        };

        Assert.Equal(limit, query.Limit);
    }
}
