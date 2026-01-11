using DotBahn.Clients.Stations.Enumerations;
using DotBahn.Clients.Stations.Models;

namespace DotBahn.Tests.Stations.Query;

public class StationsQueryTests {
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues() {
        // Arrange & Act
        var query = new StationsQuery();

        // Assert
        Assert.Null(query.Names);
        Assert.Null(query.Categories);
        Assert.Null(query.State);
        Assert.Null(query.Eva);
        Assert.Null(query.Ril);
        Assert.Equal(LogicalOperator.And, query.Operator);
        Assert.Equal(0, query.Offset);
        Assert.Equal(10000, query.Limit);
    }

    [Fact]
    public void ObjectInitializer_ShouldSetAllProperties() {
        // Arrange & Act
        var query = new StationsQuery {
            Names = ["Hamburg"],
            Categories = "1-3",
            State = FederalState.Hamburg,
            Eva = "8002549",
            Ril = "AH",
            Operator = LogicalOperator.Or,
            Offset = 10,
            Limit = 50
        };

        // Assert
        Assert.Equal(["Hamburg*"], query.Names);
        Assert.Equal("1-3", query.Categories);
        Assert.Equal(FederalState.Hamburg, query.State);
        Assert.Equal("8002549", query.Eva);
        Assert.Equal("AH", query.Ril);
        Assert.Equal(LogicalOperator.Or, query.Operator);
        Assert.Equal(10, query.Offset);
        Assert.Equal(50, query.Limit);
    }
}