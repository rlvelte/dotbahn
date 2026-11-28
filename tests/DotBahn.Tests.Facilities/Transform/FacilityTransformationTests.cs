using System.Net;
using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;
using DotBahn.Tests.Facilities.Support;
using Shouldly;

namespace DotBahn.Tests.Facilities.Transform;

public class FacilityTransformationTests {
    [Theory]
    [InlineData("8.1", "50.2", true)]
    [InlineData("8.1", "null", false)]
    [InlineData("null", "50.2", false)]
    [InlineData("null", "null", false)]
    public async Task GetFacilitiesAsync_Should_FilterByCoordinates_When_EitherIsMissing(string longitude, string latitude, bool expected) {
        using var fixture = new FacilityClientFixture();
        var json = """[{"equipmentnumber":1,"type":"ELEVATOR","state":"ACTIVE","geocoordX":LON,"geocoordY":LAT}]""".Replace("LON", longitude).Replace("LAT", latitude);
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        result.Count.ShouldBe(expected ? 1 : 0);
    }

    [Theory]
    [InlineData("0", "0", 0d, 0d)]
    [InlineData("-180", "-90", -180d, -90d)]
    [InlineData("180", "90", 180d, 90d)]
    [InlineData("1.7976931348623157E+308", "1.7976931348623157E+308", double.MaxValue, double.MaxValue)]
    public async Task GetFacilitiesAsync_Should_MapCoordinates_When_ValuesAreBoundary(string longitude, string latitude, double expectedLongitude, double expectedLatitude) {
        using var fixture = new FacilityClientFixture();
        var json = """[{"equipmentnumber":1,"geocoordX":LON,"geocoordY":LAT}]"""
            .Replace("LON", longitude).Replace("LAT", latitude);
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        var facility = result.ShouldHaveSingleItem();
        facility.Coordinates.Longitude.ShouldBe(expectedLongitude);
        facility.Coordinates.Latitude.ShouldBe(expectedLatitude);
    }

    [Theory]
    [InlineData("\"ELEVATOR\"", FacilityType.Elevator)]
    [InlineData("\"ESCALATOR\"", FacilityType.Escalator)]
    [InlineData("\"0\"", FacilityType.Elevator)]
    [InlineData("\"elevator\"", FacilityType.Unknown)]
    [InlineData("\"BOGUS\"", FacilityType.Unknown)]
    [InlineData("\"UNKNOWN\"", FacilityType.Unknown)]
    [InlineData("null", FacilityType.Unknown)]
    [InlineData("\"\"", FacilityType.Unknown)]
    [InlineData("\"   \"", FacilityType.Unknown)]
    public async Task GetFacilitiesAsync_Should_ParseType_When_ContractValueProvided(string jsonValue, FacilityType expected) {
        using var fixture = new FacilityClientFixture();
        var json = """[{"equipmentnumber":1,"type":VALUE,"geocoordX":8.1,"geocoordY":50.2}]""".Replace("VALUE", jsonValue);
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        result.ShouldHaveSingleItem().Type.ShouldBe(expected);
    }

    [Theory]
    [InlineData("\"ACTIVE\"", FacilityState.Active)]
    [InlineData("\"INACTIVE\"", FacilityState.Inactive)]
    [InlineData("\"UNKNOWN\"", FacilityState.Unknown)]
    [InlineData("\"active\"", FacilityState.Unknown)]
    [InlineData("\"BOGUS\"", FacilityState.Unknown)]
    [InlineData("null", FacilityState.Unknown)]
    [InlineData("\"\"", FacilityState.Unknown)]
    public async Task GetFacilitiesAsync_Should_ParseState_When_ContractValueProvided(string jsonValue, FacilityState expected) {
        using var fixture = new FacilityClientFixture();
        var json = """[{"equipmentnumber":1,"state":VALUE,"geocoordX":8.1,"geocoordY":50.2}]""".Replace("VALUE", jsonValue);
        fixture.Handler.RespondWith(HttpStatusCode.OK, json);

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        result.ShouldHaveSingleItem().State.ShouldBe(expected);
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_MapAllFields_When_ContractIsComplete() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            [{
              "equipmentnumber":10562421,"type":"ELEVATOR","description":"Aufzug Gleis 1",
              "state":"INACTIVE","stateExplanation":"Wartung","stationnumber":8002549,
              "geocoordX":10.0,"geocoordY":53.55,"operatorname":"DB Station&Service"
            }]
            """);

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        var facility = result.ShouldHaveSingleItem();
        facility.EquipmentNumber.ShouldBe(10562421);
        facility.Type.ShouldBe(FacilityType.Elevator);
        facility.Description.ShouldBe("Aufzug Gleis 1");
        facility.State.ShouldBe(FacilityState.Inactive);
        facility.StateExplanation.ShouldBe("Wartung");
        facility.StationNumber.ShouldBe(8002549);
        facility.Coordinates.Longitude.ShouldBe(10.0);
        facility.Coordinates.Latitude.ShouldBe(53.55);
        facility.OperatorName.ShouldBe("DB Station&Service");
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_LeaveOptionalFieldsNull_When_ContractIsMinimal() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """[{"equipmentnumber":1,"geocoordX":8.1,"geocoordY":50.2}]""");

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        var facility = result.ShouldHaveSingleItem();
        facility.Description.ShouldBeNull();
        facility.StateExplanation.ShouldBeNull();
        facility.OperatorName.ShouldBeNull();
        facility.Type.ShouldBe(FacilityType.Unknown);
        facility.State.ShouldBe(FacilityState.Unknown);
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_PreserveOrder_AndDropIncompleteFacilities() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """
            [
              {"equipmentnumber":3,"geocoordX":8.1,"geocoordY":50.2},
              {"equipmentnumber":1,"geocoordX":8.2},
              {"equipmentnumber":2,"geocoordX":8.3,"geocoordY":50.4}
            ]
            """);

        var result = await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        result.Select(facility => facility.EquipmentNumber).ShouldBe([3, 2]);
    }
}
