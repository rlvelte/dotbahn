using System.Net;
using DotBahn.Stations;
using DotBahn.Stations.Models.Enumerations;
using DotBahn.Tests.Stations.Support;
using Shouldly;

namespace DotBahn.Tests.Stations.Transform;

public class StationTransformationTests {
    [Theory]
    [InlineData(-1, StationCategory.Unknown)]
    [InlineData(0, StationCategory.Unknown)]
    [InlineData(1, StationCategory.Category1)]
    [InlineData(2, StationCategory.Category2)]
    [InlineData(4, StationCategory.Category4)]
    [InlineData(6, StationCategory.Category6)]
    [InlineData(7, StationCategory.Category7)]
    [InlineData(8, StationCategory.Unknown)]
    public async Task GetStationsAsync_Should_MapCategory_When_ValueIsBoundaryOrInterior(int category, StationCategory expected) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, $$"""{"result":[{"number":1,"name":"S","category":{{category}},"ril100Identifiers":[],"evaNumbers":[]}]}""");

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        result.ShouldHaveSingleItem().Category.ShouldBe(expected);
    }

    [Theory]
    [InlineData("null", null, null)]
    [InlineData("[]", null, null)]
    [InlineData("[13.4]", null, null)]
    [InlineData("[13.4,52.5]", 13.4, 52.5)]
    [InlineData("[13.4,52.5,99]", 13.4, 52.5)]
    public async Task GetStationsAsync_Should_MapCoordinates_When_CountIsBoundaryOrInterior(string coordinates, double? longitude, double? latitude) {
        using var fixture = new StationClientFixture();
        var json = """{"result":[{"number":1,"name":"S","category":1,"ril100Identifiers":[],"evaNumbers":[{"number":8002549,"isMain":true,"geographicCoordinates":{"type":"Point","coordinates":COORDS}}]}]}""".Replace("COORDS", coordinates);
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        var station = result.ShouldHaveSingleItem();
        station.Coordinates.HasValue.ShouldBe(longitude.HasValue);
        station.Coordinates?.Longitude.ShouldBe(longitude.GetValueOrDefault());
        station.Coordinates?.Latitude.ShouldBe(latitude.GetValueOrDefault());
    }

    [Fact]
    public async Task GetStationsAsync_Should_LeaveCoordinatesNull_When_NoEvaNumbersExist() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"result":[{"number":1,"name":"S","category":1,"ril100Identifiers":[],"evaNumbers":[]}]}""");

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        var station = result.ShouldHaveSingleItem();
        station.PrimaryEva.ShouldBeNull();
        station.Coordinates.ShouldBeNull();
    }

    [Fact]
    public async Task GetStationsAsync_Should_UseMainEvaCoordinates_When_OtherEvaHasCoordinates() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            {"result":[{"number":1,"name":"S","category":1,"ril100Identifiers":[],"evaNumbers":[
              {"number":1,"isMain":false,"geographicCoordinates":{"coordinates":[10.0,50.0]}},
              {"number":2,"isMain":true}
            ]}]}
            """);

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        result.ShouldHaveSingleItem().Coordinates.ShouldBeNull();
    }

    [Fact]
    public async Task GetStationsAsync_Should_MapAllFields_When_ContractIsComplete() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            {"offset":0,"limit":10,"total":1,"result":[{
              "number":123,"name":"Hamburg Hbf","category":2,
              "mailingAddress":{"city":"Hamburg","zipcode":"20095","street":"Hachmannplatz 16"},
              "regionalBereich":{"number":101,"name":"Hamburg","shortName":"RB"},
              "ril100Identifiers":[{"rilIdentifier":"AH","isMain":"JA"},{"rilIdentifier":"AHS","isMain":false}],
              "evaNumbers":[{"number":8002549,"isMain":true,"geographicCoordinates":{"type":"Point","coordinates":[10.0,53.55]}},{"number":8002548,"isMain":false}],
              "hasParking":"YES","hasBicycleParking":true,"hasPublicFacilities":"NO","hasLockerSystem":false,
              "hasStepFreeAccess":"1","hasSteplessAccess":"FALSE","hasTravelCenter":true,
              "hasTravelNecessities":"maybe","hasMobilityService":true,"hasWiFi":true
            }]}
            """);

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        var station = result.ShouldHaveSingleItem();
        station.Number.ShouldBe(123);
        station.Name.ShouldBe("Hamburg Hbf");
        station.Category.ShouldBe(StationCategory.Category2);
        station.Address!.DisplayAddress.ShouldBe("Hachmannplatz 16, 20095 Hamburg");
        station.RegionalArea!.Number.ShouldBe(101);
        station.RegionalArea.Name.ShouldBe("Hamburg");
        station.RegionalArea.ShortName.ShouldBe("RB");
        station.PrimaryRil100!.Identifier.ShouldBe("AH");
        station.PrimaryRil100.IsMain.ShouldBeTrue();
        station.PrimaryEva!.Number.ShouldBe(8002549);
        station.Coordinates!.Value.Longitude.ShouldBe(10.0);
        station.Coordinates!.Value.Latitude.ShouldBe(53.55);
        station.Services.HasParking.ShouldBeTrue();
        station.Services.HasBicycleParking.ShouldBeTrue();
        station.Services.HasPublicFacilities.ShouldBeFalse();
        station.Services.HasLockerSystem.ShouldBeFalse();
        station.Services.HasStepFreeAccess.ShouldBeTrue();
        station.Services.HasSteplessAccess.ShouldBeFalse();
        station.Services.HasTravelCenter.ShouldBeTrue();
        station.Services.HasTravelNecessities.ShouldBeFalse();
        station.Services.HasMobilityService.ShouldBeTrue();
        station.Services.HasWiFi.ShouldBeTrue();
    }

    [Fact]
    public async Task GetStationsAsync_Should_LeaveOptionalPartsNull_When_ContractIsMinimal() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"result":[{"number":1,"name":"S","category":1,"ril100Identifiers":[],"evaNumbers":[]}]}""");

        var result = await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        var station = result.ShouldHaveSingleItem();
        station.Address.ShouldBeNull();
        station.RegionalArea.ShouldBeNull();
        station.Ril100Identifiers.ShouldBeEmpty();
        station.EvaNumbers.ShouldBeEmpty();
        station.PrimaryRil100.ShouldBeNull();
        station.PrimaryEva.ShouldBeNull();
    }
}
