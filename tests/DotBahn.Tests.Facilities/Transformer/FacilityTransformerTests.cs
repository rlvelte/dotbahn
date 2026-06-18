using DotBahn.Facilities;
using DotBahn.Facilities.Contracts;
using DotBahn.Facilities.Enumerations;

namespace DotBahn.Tests.Facilities.Transformer;

public class FacilityTransformerTests {
    private readonly FacilityTransformer _transformer = new();

    [Theory]
    [InlineData(null, 53.5)]
    [InlineData(10.0, null)]
    [InlineData(null, null)]
    public void Transform_NullCoordinates_FiltersOut(double? longitude, double? latitude) {
        var contract = new FacilityContract {
            EquipmentNumber = 100,
            Type = "ELEVATOR",
            State = "ACTIVE",
            Longitude = longitude,
            Latitude = latitude
        };

        var result = _transformer.Transform([contract]);

        Assert.Empty(result);
    }

    [Fact]
    public void Transform_ValidContract_MapsAllFieldsIncludingCoordinates() {
        var contract = new FacilityContract {
            EquipmentNumber = 200,
            Type = "ESCALATOR",
            Description = "Platform 1 Escalator",
            State = "ACTIVE",
            StateExplanation = "Fully operational",
            StationNumber = 8000001,
            Longitude = 10.0,
            Latitude = 53.5,
            OperatorName = "DB Station&Service AG"
        };

        var result = _transformer.Transform([contract]).ToList();

        Assert.Single(result);
        var facility = result[0];
        Assert.Equal(200, facility.EquipmentNumber);
        Assert.Equal(FacilityType.Escalator, facility.Type);
        Assert.Equal("Platform 1 Escalator", facility.Description);
        Assert.Equal(FacilityState.Active, facility.State);
        Assert.Equal("Fully operational", facility.StateExplanation);
        Assert.Equal(8000001, facility.StationNumber);
        Assert.NotNull(facility.Coordinates);
        Assert.Equal(10.0, facility.Coordinates.Longitude);
        Assert.Equal(53.5, facility.Coordinates.Latitude);
        Assert.Equal("DB Station&Service AG", facility.OperatorName);
    }
}
