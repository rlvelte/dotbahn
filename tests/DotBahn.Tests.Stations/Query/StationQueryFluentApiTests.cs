using DotBahn.Stations;
using DotBahn.Stations.Enumerations;

namespace DotBahn.Tests.Stations.Query;

public class StationQueryFluentApiTests {
    [Fact]
    public void InFederalState_ShouldSetStateAndReturnQuery() {
        var query = new StationQuery();

        var result = query.InFederalState(FederalState.Hamburg);

        Assert.Same(query, result);
        Assert.Equal(FederalState.Hamburg, query.State);
    }

    [Fact]
    public void WithEva_ShouldSetEvaAndReturnQuery() {
        var query = new StationQuery();

        var result = query.WithEva("8002549");

        Assert.Same(query, result);
        Assert.Equal("8002549", query.Eva);
    }

    [Fact]
    public void WithRil_ShouldSetRilAndReturnQuery() {
        var query = new StationQuery();

        var result = query.WithRil("AH");

        Assert.Same(query, result);
        Assert.Equal("AH", query.Ril);
    }

    [Fact]
    public void CombineAs_ShouldSetOperatorAndReturnQuery() {
        var query = new StationQuery();

        var result = query.CombineAs(LogicalOperator.Or);

        Assert.Same(query, result);
        Assert.Equal(LogicalOperator.Or, query.Operator);
    }

    [Fact]
    public void Skip_ShouldSetOffsetAndReturnQuery() {
        var query = new StationQuery();

        var result = query.Skip(50);

        Assert.Same(query, result);
        Assert.Equal(50, query.Offset);
    }

    [Fact]
    public void LimitTo_ShouldSetLimitAndReturnQuery() {
        var query = new StationQuery();

        var result = query.LimitTo(100);

        Assert.Same(query, result);
        Assert.Equal(100, query.Limit);
    }

    [Fact]
    public void FluentApi_ComplexChaining_ShouldSetAllProperties() {
        var query = new StationQuery()
                    .WithNames("Hamburg", "Berlin")
                    .WithCategories("1-3")
                    .InFederalState(FederalState.Hamburg)
                    .WithEva("8002549")
                    .WithRil("AH")
                    .CombineAs(LogicalOperator.Or)
                    .Skip(10)
                    .LimitTo(50);

        Assert.Equal(["Hamburg*", "Berlin*"], query.Names!);
        Assert.Equal("1-3", query.Categories);
        Assert.Equal(FederalState.Hamburg, query.State);
        Assert.Equal("8002549", query.Eva);
        Assert.Equal("AH", query.Ril);
        Assert.Equal(LogicalOperator.Or, query.Operator);
        Assert.Equal(10, query.Offset);
        Assert.Equal(50, query.Limit);
    }

    [Fact]
    public void FluentApi_PartialChaining_ShouldSetOnlySpecifiedProperties() {
        var query = new StationQuery()
                    .WithNames("Hamburg")
                    .WithCategories("1")
                    .LimitTo(5);

        Assert.Equal(["Hamburg*"], query.Names!);
        Assert.Equal("1", query.Categories);
        Assert.Null(query.State);
        Assert.Null(query.Eva);
        Assert.Null(query.Ril);
        Assert.Equal(LogicalOperator.And, query.Operator);
        Assert.Equal(0, query.Offset);
        Assert.Equal(5, query.Limit);
    }
}
