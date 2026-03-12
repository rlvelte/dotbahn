using DotBahn.Clients.Stations.Contracts;
using DotBahn.Clients.Stations.Transformer;
using DotBahn.Data.Stations.Enumerations;

namespace DotBahn.Tests.Stations.Transformer;

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

    // --- Category ---

    [Theory]
    [InlineData(1, StationCategory.Category1)]
    [InlineData(3, StationCategory.Category3)]
    [InlineData(7, StationCategory.Category7)]
    public void Transform_ValidCategory_MapsToEnum(int cat, StationCategory expected) {
        // Act
        var result = _transformer.Transform(Wrap(Minimal(cat))).First();

        // Assert
        Assert.Equal(expected, result.Category);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(-1)]
    public void Transform_OutOfRangeCategory_MapsToUnknown(int cat) {
        // Act
        var result = _transformer.Transform(Wrap(Minimal(cat))).First();

        // Assert
        Assert.Equal(StationCategory.Unknown, result.Category);
    }

    // --- Address ---

    [Fact]
    public void Transform_NullAddress_ReturnsNullAddress() {
        // Arrange
        var contract = Minimal() with { MailingAddress = null };

        // Act
        var result = _transformer.Transform(Wrap(contract)).First();

        // Assert
        Assert.Null(result.Address);
    }

    [Fact]
    public void Transform_WithAddress_MapsFields() {
        // Arrange
        var contract = Minimal() with {
            MailingAddress = new MailingAddressContract {
                Street = "Hauptstraße 1",
                ZipCode = "20095",
                City = "Hamburg"
            }
        };

        // Act
        var result = _transformer.Transform(Wrap(contract)).First();

        // Assert
        Assert.NotNull(result.Address);
        Assert.Equal("Hauptstraße 1", result.Address.Street);
        Assert.Equal("20095", result.Address.ZipCode);
        Assert.Equal("Hamburg", result.Address.City);
    }

    // --- RegionalArea ---

    [Fact]
    public void Transform_NullRegionalArea_ReturnsNullRegionalArea() {
        // Arrange
        var contract = Minimal() with { RegionalArea = null };

        // Act
        var result = _transformer.Transform(Wrap(contract)).First();

        // Assert
        Assert.Null(result.RegionalArea);
    }

    [Fact]
    public void Transform_WithRegionalArea_MapsFields() {
        // Arrange
        var contract = Minimal() with {
            RegionalArea = new RegionalAreaContract {
                Number = 10,
                Name = "Nord",
                ShortName = "N"
            }
        };

        // Act
        var result = _transformer.Transform(Wrap(contract)).First();

        // Assert
        Assert.NotNull(result.RegionalArea);
        Assert.Equal(10, result.RegionalArea.Number);
        Assert.Equal("Nord", result.RegionalArea.Name);
        Assert.Equal("N", result.RegionalArea.ShortName);
    }

    // --- Coordinates ---

    [Fact]
    public void Transform_EvaWithNullCoordinates_ReturnsNullCoordinatesOnEva() {
        // Arrange
        var contract = Minimal() with {
            EvaNumbers = [new EvaNumberContract { Number = 8000001, IsMain = true, GeographicCoordinates = null }]
        };

        // Act
        var result = _transformer.Transform(Wrap(contract)).First();

        // Assert
        Assert.Null(result.EvaNumbers.First().Coordinates);
    }

    [Fact]
    public void Transform_EvaWithSingleCoordinate_ReturnsNullCoordinates() {
        // Arrange
        var contract = Minimal() with {
            EvaNumbers = [new EvaNumberContract {
                Number = 8000001,
                IsMain = true,
                GeographicCoordinates = new GeographicCoordinatesContract { Coordinates = [10.0] }
            }]
        };

        // Act
        var result = _transformer.Transform(Wrap(contract)).First();

        // Assert
        Assert.Null(result.EvaNumbers.First().Coordinates);
    }

    [Fact]
    public void Transform_EvaWithTwoCoordinates_MapsLongitudeAndLatitude() {
        // Arrange
        var contract = Minimal() with {
            EvaNumbers = [new EvaNumberContract {
                Number = 8000001,
                IsMain = true,
                GeographicCoordinates = new GeographicCoordinatesContract { Coordinates = [10.0, 53.5] }
            }]
        };

        // Act
        var result = _transformer.Transform(Wrap(contract)).First();
        var coords = result.EvaNumbers.First().Coordinates;

        // Assert
        Assert.NotNull(coords);
        Assert.Equal(10.0, coords.Longitude);
        Assert.Equal(53.5, coords.Latitude);
    }
}
