using DotBahn.Clients.Facilities.Models;

namespace DotBahn.Tests.Facilities.Query;

public class FacilitiesQueryEquipmentNumbersTests {
    [Fact]
    public void EquipmentNumbers_WithSingleNumber_ShouldSetValue() {
        // Arrange & Act
        var query = new FacilitiesQuery {
            EquipmentNumbers = ["10562421"]
        };

        // Assert
        Assert.Single(query.EquipmentNumbers);
        Assert.Equal("10562421", query.EquipmentNumbers[0]);
    }

    [Fact]
    public void EquipmentNumbers_WithMultipleNumbers_ShouldSetAllValues() {
        // Arrange
        var query = new FacilitiesQuery();
        var numbers = new[] { "10562421", "10562422", "10562423" };

        // Act
        query.EquipmentNumbers = numbers;

        // Assert
        Assert.Equal(3, query.EquipmentNumbers.Length);
        Assert.Equal(numbers, query.EquipmentNumbers);
    }

    [Fact]
    public void EquipmentNumbers_WithEmptyArray_ShouldSetEmptyArray() {
        // Arrange & Act
        var query = new FacilitiesQuery {
            EquipmentNumbers = []
        };

        // Assert
        Assert.NotNull(query.EquipmentNumbers);
        Assert.Empty(query.EquipmentNumbers);
    }

    [Fact]
    public void EquipmentNumbers_WithNull_ShouldSetNull() {
        // Arrange
        var query = new FacilitiesQuery { EquipmentNumbers = ["10562421"] };

        // Act
        query.EquipmentNumbers = null;

        // Assert
        Assert.Null(query.EquipmentNumbers);
    }

    [Fact]
    public void WithEquipmentNumbers_WithSingleParameter_ShouldSetArray() {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        var result = query.WithEquipmentNumbers("10562421");

        // Assert
        Assert.Same(query, result);
        Assert.Single(query.EquipmentNumbers!);
        Assert.Equal("10562421", query.EquipmentNumbers![0]);
    }

    [Fact]
    public void WithEquipmentNumbers_WithMultipleParameters_ShouldSetAllValues() {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        var result = query.WithEquipmentNumbers("10562421", "10562422", "10562423");

        // Assert
        Assert.Same(query, result);
        Assert.Equal(3, query.EquipmentNumbers!.Length);
        Assert.Equal(["10562421", "10562422", "10562423"], query.EquipmentNumbers);
    }

    [Fact]
    public void WithEquipmentNumbers_CalledMultipleTimes_ShouldOverwritePreviousValue() {
        // Arrange
        var query = new FacilitiesQuery();

        // Act
        query.WithEquipmentNumbers("10562421")
             .WithEquipmentNumbers("10562422", "10562423");

        // Assert
        Assert.Equal(2, query.EquipmentNumbers!.Length);
        Assert.Equal(["10562422", "10562423"], query.EquipmentNumbers);
    }
}