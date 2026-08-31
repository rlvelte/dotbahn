using DotBahn.Facilities;

namespace DotBahn.Facilities.Tests.Query;

public class FacilityQueryEquipmentNumbersTests {
    [Fact]
    public void EquipmentNumbers_WithSingleNumber_ShouldSetValue() {
        var query = new FacilityQuery {
            EquipmentNumbers = ["10562421"]
        };

        Assert.Single(query.EquipmentNumbers);
        Assert.Equal("10562421", query.EquipmentNumbers.First());
    }

    [Fact]
    public void EquipmentNumbers_WithMultipleNumbers_ShouldSetAllValues() {
        var query = new FacilityQuery();
        var numbers = new[] { "10562421", "10562422", "10562423" };

        query.EquipmentNumbers = numbers;

        Assert.Equal(3, query.EquipmentNumbers.Count());
        Assert.Equal(numbers, query.EquipmentNumbers);
    }

    [Fact]
    public void EquipmentNumbers_WithEmptyArray_ShouldSetEmptyArray() {
        var query = new FacilityQuery {
            EquipmentNumbers = []
        };

        Assert.NotNull(query.EquipmentNumbers);
        Assert.Empty(query.EquipmentNumbers);
    }

    [Fact]
    public void EquipmentNumbers_WithNull_ShouldSetNull() {
        var query = new FacilityQuery { EquipmentNumbers = ["10562421"] };

        query.EquipmentNumbers = null!;

        Assert.Null(query.EquipmentNumbers);
    }

    [Fact]
    public void WithEquipmentNumbers_WithSingleParameter_ShouldSetArray() {
        var query = new FacilityQuery();

        var result = query.WithEquipmentNumbers("10562421");

        Assert.Same(query, result);
        Assert.Single(query.EquipmentNumbers);
        Assert.Equal("10562421", query.EquipmentNumbers.First());
    }

    [Fact]
    public void WithEquipmentNumbers_WithMultipleParameters_ShouldSetAllValues() {
        var query = new FacilityQuery();

        var result = query.WithEquipmentNumbers("10562421", "10562422", "10562423");

        Assert.Same(query, result);
        Assert.Equal(3, query.EquipmentNumbers.Count());
        Assert.Equal(["10562421", "10562422", "10562423"], query.EquipmentNumbers);
    }

    [Fact]
    public void WithEquipmentNumbers_CalledMultipleTimes_ShouldOverwritePreviousValue() {
        var query = new FacilityQuery();

        query.WithEquipmentNumbers("10562421")
             .WithEquipmentNumbers("10562422", "10562423");

        Assert.Equal(2, query.EquipmentNumbers.Count());
        Assert.Equal(["10562422", "10562423"], query.EquipmentNumbers);
    }
}
