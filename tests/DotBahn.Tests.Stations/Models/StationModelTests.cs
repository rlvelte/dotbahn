using DotBahn.Common.Models;
using DotBahn.Stations.Models;
using DotBahn.Stations.Models.Enumerations;
using Shouldly;

namespace DotBahn.Tests.Stations.Models;

public class StationModelTests {
    [Fact]
    public void Constructor_Should_UseEmptyCollections_When_NotProvided() {
        var sut = new Station {
            Number = 1,
            Name = "Test",
            Category = StationCategory.Category1,
            Services = new StationServices()
        };

        sut.Ril100Identifiers.ShouldBeEmpty();
        sut.EvaNumbers.ShouldBeEmpty();
    }

    [Fact]
    public void PrimaryRil100_Should_BeNull_When_NoIdentifiersExist() {
        var sut = CreateStation();

        sut.PrimaryRil100.ShouldBeNull();
    }

    [Theory]
    [InlineData(false, false, "A")]
    [InlineData(false, true, "B")]
    [InlineData(true, false, "A")]
    [InlineData(true, true, "A")]
    public void PrimaryRil100_Should_PreferMain_When_IdentifiersExist(bool firstIsMain, bool secondIsMain, string expected) {
        var sut = CreateStation(rils: [
            new Ril100Identifier { Identifier = "A", IsMain = firstIsMain },
            new Ril100Identifier { Identifier = "B", IsMain = secondIsMain }
        ]);

        sut.PrimaryRil100!.Identifier.ShouldBe(expected);
    }

    [Fact]
    public void PrimaryEva_Should_BeNull_When_NoEvaNumbersExist() {
        var sut = CreateStation();

        sut.PrimaryEva.ShouldBeNull();
    }

    [Theory]
    [InlineData(false, false, 1L)]
    [InlineData(false, true, 2L)]
    [InlineData(true, false, 1L)]
    [InlineData(true, true, 1L)]
    public void PrimaryEva_Should_PreferMain_When_EvaNumbersExist(bool firstIsMain, bool secondIsMain, long expected) {
        var sut = CreateStation(evas: [
            new EvaNumber { Number = 1, IsMain = firstIsMain },
            new EvaNumber { Number = 2, IsMain = secondIsMain }
        ]);

        sut.PrimaryEva!.Number.ShouldBe(expected);
    }

    [Fact]
    public void Coordinates_Should_UsePrimaryEva_When_MainEvaHasCoordinates() {
        var coordinates = new Coordinates { Longitude = 10.0, Latitude = 53.55 };
        var sut = CreateStation(evas: [
            new EvaNumber { Number = 1, IsMain = false, Coordinates = new Coordinates { Longitude = 20.0, Latitude = 40.0 } },
            new EvaNumber { Number = 2, IsMain = true, Coordinates = coordinates }
        ]);

        sut.Coordinates.ShouldBe(coordinates);
    }

    [Fact]
    public void Coordinates_Should_BeNull_When_PrimaryEvaHasNoCoordinates() {
        var sut = CreateStation(evas: [
            new EvaNumber { Number = 1, IsMain = false, Coordinates = new Coordinates { Longitude = 20.0, Latitude = 40.0 } },
            new EvaNumber { Number = 2, IsMain = true }
        ]);

        sut.Coordinates.ShouldBeNull();
    }

    [Theory]
    [InlineData("Hachmannplatz 16", "20095", "Hamburg", "Hachmannplatz 16, 20095 Hamburg")]
    [InlineData("", "", "", ",  ")]
    [InlineData("Bahnhofstraße 1", "10115", "Berlin", "Bahnhofstraße 1, 10115 Berlin")]
    public void DisplayAddress_Should_FormatComponents_When_AddressIsCreated(string street, string zipCode, string city, string expected) {
        var sut = new StationAddress { Street = street, ZipCode = zipCode, City = city };

        sut.DisplayAddress.ShouldBe(expected);
    }

    private static Station CreateStation(Ril100Identifier[]? rils = null, EvaNumber[]? evas = null) => new() {
        Number = 1,
        Name = "Test",
        Category = StationCategory.Category1,
        Services = new StationServices(),
        Ril100Identifiers = rils ?? [],
        EvaNumbers = evas ?? []
    };
}
