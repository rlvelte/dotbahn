using DotBahn.Stations.Internal.Contracts;
using DotBahn.Stations.Internal.Transformers;
using DotBahn.Stations.Models.Enumerations;

namespace DotBahn.Stations.Tests.Transformer;

public class StationTransformerTests {
    private readonly StationTransformer _transformer = new();

    private static StationsResponseContract Wrap(StationContract station) => new() {
        Stations = [station]
    };

    private static StationContract Minimal(int category = 1) => new() {
        Number = 1,
        Name = "Test",
        Category = category
    };

    [Theory]
    [InlineData(1, StationCategory.Category1)]
    [InlineData(3, StationCategory.Category3)]
    [InlineData(7, StationCategory.Category7)]
    public void Transform_ValidCategory_MapsToEnum(int cat, StationCategory expected) {
        var result = _transformer.Transform(Wrap(Minimal(cat))).First();

        Assert.Equal(expected, result.Category);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(-1)]
    public void Transform_OutOfRangeCategory_MapsToUnknown(int cat) {
        var result = _transformer.Transform(Wrap(Minimal(cat))).First();

        Assert.Equal(StationCategory.Unknown, result.Category);
    }

    [Fact]
    public void Transform_NullAddress_ReturnsNullAddress() {
        var contract = Minimal() with { MailingAddress = null };

        var result = _transformer.Transform(Wrap(contract)).First();

        Assert.Null(result.Address);
    }

    [Fact]
    public void Transform_WithAddress_MapsFields() {
        var contract = Minimal() with {
            MailingAddress = new MailingAddressContract {
                Street = "Hauptstraße 1",
                ZipCode = "20095",
                City = "Hamburg"
            }
        };

        var result = _transformer.Transform(Wrap(contract)).First();

        Assert.NotNull(result.Address);
        Assert.Equal("Hauptstraße 1", result.Address.Street);
        Assert.Equal("20095", result.Address.ZipCode);
        Assert.Equal("Hamburg", result.Address.City);
    }


    [Fact]
    public void Transform_NullRegionalArea_ReturnsNullRegionalArea() {
        var contract = Minimal() with { RegionalArea = null };

        var result = _transformer.Transform(Wrap(contract)).First();

        Assert.Null(result.RegionalArea);
    }

    [Fact]
    public void Transform_WithRegionalArea_MapsFields() {
        var contract = Minimal() with {
            RegionalArea = new RegionalAreaContract {
                Number = 10,
                Name = "Nord",
                ShortName = "N"
            }
        };

        var result = _transformer.Transform(Wrap(contract)).First();

        Assert.NotNull(result.RegionalArea);
        Assert.Equal(10, result.RegionalArea.Number);
        Assert.Equal("Nord", result.RegionalArea.Name);
        Assert.Equal("N", result.RegionalArea.ShortName);
    }


    [Fact]
    public void Transform_EvaWithNullCoordinates_ReturnsNullCoordinatesOnEva() {
        var contract = Minimal() with {
            EvaNumbers = [new EvaNumberContract { Number = 8000001, IsMain = true, GeographicCoordinates = null }]
        };

        var result = _transformer.Transform(Wrap(contract)).First();

        Assert.Null(result.EvaNumbers.First().Coordinates);
    }

    [Fact]
    public void Transform_EvaWithZeroCoordinates_ReturnsNullCoordinates() {
        var contract = Minimal() with {
            EvaNumbers = [new EvaNumberContract {
                Number = 8000001,
                IsMain = true,
                GeographicCoordinates = new GeographicCoordinatesContract { Coordinates = [] }
            }]
        };

        var result = _transformer.Transform(Wrap(contract)).First();

        Assert.Null(result.EvaNumbers.First().Coordinates);
    }

    [Fact]
    public void Transform_EvaWithSingleCoordinate_ReturnsNullCoordinates() {
        var contract = Minimal() with {
            EvaNumbers = [new EvaNumberContract {
                Number = 8000001,
                IsMain = true,
                GeographicCoordinates = new GeographicCoordinatesContract { Coordinates = [10.0] }
            }]
        };

        var result = _transformer.Transform(Wrap(contract)).First();

        Assert.Null(result.EvaNumbers.First().Coordinates);
    }

    [Fact]
    public void Transform_EvaWithTwoCoordinates_MapsLongitudeAndLatitude() {
        var contract = Minimal() with {
            EvaNumbers = [new EvaNumberContract {
                Number = 8000001,
                IsMain = true,
                GeographicCoordinates = new GeographicCoordinatesContract { Coordinates = [10.0, 53.5] }
            }]
        };

        var result = _transformer.Transform(Wrap(contract)).First();
        var coords = result.EvaNumbers.First().Coordinates;

        Assert.NotNull(coords);
        Assert.Equal(10.0, coords.Longitude);
        Assert.Equal(53.5, coords.Latitude);
    }


    [Fact]
    public void Transform_AfterConstruction_DoesNotThrow() {
        var transformer = new StationTransformer();

        var result = transformer.Transform(Wrap(Minimal()));

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
