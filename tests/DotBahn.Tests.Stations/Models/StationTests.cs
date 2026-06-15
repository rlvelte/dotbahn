using DotBahn.Data.Shared.Models;
using DotBahn.Data.Stations.Enumerations;
using DotBahn.Data.Stations.Models;

namespace DotBahn.Tests.Stations.Models;

public class StationTests {
    private static Station Make(
        IEnumerable<Ril100Identifier>? ril100 = null,
        IEnumerable<EvaNumber>? evaNumbers = null) => new() {
            Number = 1,
            Name = "Test",
            Category = StationCategory.Category1,
            Services = new StationServices(),
            Ril100Identifiers = ril100 ?? [],
            EvaNumbers = evaNumbers ?? []
        };

    // --- PrimaryRil100 ---

    [Fact]
    public void PrimaryRil100_Empty_ReturnsNull() {
        // Arrange
        var station = Make();

        // Assert
        Assert.Null(station.PrimaryRil100);
    }

    [Fact]
    public void PrimaryRil100_MainExists_ReturnsMain() {
        // Arrange
        var main = new Ril100Identifier { Identifier = "HH", IsMain = true };
        var other = new Ril100Identifier { Identifier = "HH2", IsMain = false };
        var station = Make(ril100: [other, main]);

        // Assert
        Assert.Same(main, station.PrimaryRil100);
    }

    [Fact]
    public void PrimaryRil100_NoMain_ReturnsFallbackFirst() {
        // Arrange
        var first = new Ril100Identifier { Identifier = "HH", IsMain = false };
        var second = new Ril100Identifier { Identifier = "HH2", IsMain = false };
        var station = Make(ril100: [first, second]);

        // Assert
        Assert.Same(first, station.PrimaryRil100);
    }

    // --- PrimaryEva ---

    [Fact]
    public void PrimaryEva_Empty_ReturnsNull() {
        // Arrange
        var station = Make();

        // Assert
        Assert.Null(station.PrimaryEva);
    }

    [Fact]
    public void PrimaryEva_MainExists_ReturnsMain() {
        // Arrange
        var main = new EvaNumber { Number = 8000105, IsMain = true };
        var other = new EvaNumber { Number = 8000106, IsMain = false };
        var station = Make(evaNumbers: [other, main]);

        // Assert
        Assert.Same(main, station.PrimaryEva);
    }

    [Fact]
    public void PrimaryEva_NoMain_ReturnsFallbackFirst() {
        // Arrange
        var first = new EvaNumber { Number = 8000105, IsMain = false };
        var second = new EvaNumber { Number = 8000106, IsMain = false };
        var station = Make(evaNumbers: [first, second]);

        // Assert
        Assert.Same(first, station.PrimaryEva);
    }

    // --- Coordinates ---

    [Fact]
    public void Coordinates_NoEva_ReturnsNull() {
        // Arrange
        var station = Make();

        // Assert
        Assert.Null(station.Coordinates);
    }

    [Fact]
    public void Coordinates_PrimaryEvaHasNoCoordinates_ReturnsNull() {
        // Arrange
        var eva = new EvaNumber { Number = 8000105, IsMain = true, Coordinates = null };

        // Assert
        Assert.Null(Make(evaNumbers: [eva]).Coordinates);
    }

    [Fact]
    public void Coordinates_PrimaryEvaHasCoordinates_ReturnsCoordinates() {
        // Arrange
        var coords = new Coordinates { Longitude = 10.0, Latitude = 53.5 };
        var eva = new EvaNumber { Number = 8000105, IsMain = true, Coordinates = coords };

        // Assert
        Assert.Same(coords, Make(evaNumbers: [eva]).Coordinates);
    }
}
