using DotBahn.Clients.Facilities.Query;
using DotBahn.Data.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Query;

public class FacilitiesQueryFluentApiTests {
    [Fact]
    public void FluentApi_CompleteChaining_ShouldSetAllProperties() {
        // Arrange & Act
        var query = new FacilitiesQuery()
                    .WithType(FacilityType.Elevator)
                    .WithState(FacilityState.Active)
                    .WithEquipmentNumbers("10562421", "10562422")
                    .AtStation(8002549);

        // Assert
        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Active, query.State);
        Assert.Equal(["10562421", "10562422"], query.EquipmentNumbers!);
        Assert.Equal("8002549", query.StationId);
    }

    [Fact]
    public void FluentApi_PartialChaining_ShouldSetOnlySpecifiedProperties() {
        // Arrange & Act
        var query = new FacilitiesQuery()
                    .WithType(FacilityType.Escalator)
                    .AtStation(8000105);

        // Assert
        Assert.Equal(FacilityType.Escalator, query.Type);
        Assert.Null(query.State);
        Assert.Null(query.EquipmentNumbers);
        Assert.Equal("8000105", query.StationId);
    }

    [Fact]
    public void FluentApi_ChainingSingleMethod_ShouldReturnSameInstance() {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        var result1 = query.WithType(FacilityType.Elevator);
        var result2 = result1.WithState(FacilityState.Active);
        var result3 = result2.WithEquipmentNumbers("10562421");
        var result4 = result3.AtStation(8002549);

        // Assert
        Assert.Same(query, result1);
        Assert.Same(query, result2);
        Assert.Same(query, result3);
        Assert.Same(query, result4);
    }

    [Fact]
    public void FluentApi_MixedWithObjectInitializer_ShouldCombineBothApproaches() {
        // Arrange & Act
        var query = new FacilitiesQuery {
                Type = FacilityType.Elevator
            }.WithState(FacilityState.Active)
             .AtStation(8002549);

        // Assert
        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Active, query.State);
        Assert.Equal("8002549", query.StationId);
    }

    [Fact]
    public void FluentApi_OverwritingValues_ShouldUseLastSetValue() {
        // Arrange & Act
        var query = new FacilitiesQuery()
                    .WithType(FacilityType.Elevator)
                    .WithType(FacilityType.Escalator)
                    .AtStation(8002549)
                    .AtStation(8000105);

        // Assert
        Assert.Equal(FacilityType.Escalator, query.Type);
        Assert.Equal("8000105", query.StationId);
    }
}