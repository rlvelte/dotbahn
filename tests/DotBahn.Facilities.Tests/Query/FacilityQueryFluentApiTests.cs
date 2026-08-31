using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Facilities.Tests.Query;

public class FacilityQueryFluentApiTests {
    [Fact]
    public void FluentApi_CompleteChaining_ShouldSetAllProperties() {
        var query = new FacilityQuery()
                    .WithType(FacilityType.Elevator)
                    .WithState(FacilityState.Active)
                    .WithEquipmentNumbers("10562421", "10562422")
                    .AtStation(8002549);

        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Active, query.State);
        Assert.Equal(["10562421", "10562422"], query.EquipmentNumbers);
        Assert.Equal("8002549", query.StationId);
    }

    [Fact]
    public void FluentApi_PartialChaining_ShouldSetOnlySpecifiedProperties() {
        var query = new FacilityQuery()
                    .WithType(FacilityType.Escalator)
                    .AtStation(8000105);

        Assert.Equal(FacilityType.Escalator, query.Type);
        Assert.Null(query.State);
        Assert.Empty(query.EquipmentNumbers);
        Assert.Equal("8000105", query.StationId);
    }

    [Fact]
    public void FluentApi_ChainingSingleMethod_ShouldReturnSameInstance() {
        var query = new FacilityQuery();

        var result1 = query.WithType(FacilityType.Elevator);
        var result2 = result1.WithState(FacilityState.Active);
        var result3 = result2.WithEquipmentNumbers("10562421");
        var result4 = result3.AtStation(8002549);

        Assert.Same(query, result1);
        Assert.Same(query, result2);
        Assert.Same(query, result3);
        Assert.Same(query, result4);
    }

    [Fact]
    public void FluentApi_MixedWithObjectInitializer_ShouldCombineBothApproaches() {
        var query = new FacilityQuery {
            Type = FacilityType.Elevator
        }.WithState(FacilityState.Active)
             .AtStation(8002549);

        Assert.Equal(FacilityType.Elevator, query.Type);
        Assert.Equal(FacilityState.Active, query.State);
        Assert.Equal("8002549", query.StationId);
    }

    [Fact]
    public void FluentApi_OverwritingValues_ShouldUseLastSetValue() {
        var query = new FacilityQuery()
                    .WithType(FacilityType.Elevator)
                    .WithType(FacilityType.Escalator)
                    .AtStation(8002549)
                    .AtStation(8000105);

        Assert.Equal(FacilityType.Escalator, query.Type);
        Assert.Equal("8000105", query.StationId);
    }
}
