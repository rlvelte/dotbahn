using System.Globalization;
using System.Net;
using DotBahn.Facilities;
using DotBahn.Facilities.Models.Enumerations;
using DotBahn.Tests.Facilities.Support;
using Shouldly;

namespace DotBahn.Tests.Facilities.Query;

public class FacilityQueryParameterTests {
    private const string EmptyResponse = "[]";

    [Theory]
    [InlineData(FacilityType.Elevator, "type=ELEVATOR")]
    [InlineData(FacilityType.Escalator, "type=ESCALATOR")]
    [InlineData(FacilityType.Unknown, "type=Unknown")]
    public async Task GetFacilitiesAsync_Should_FormatType_When_TypeProvided(FacilityType type, string expected) {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery().WithType(type), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Theory]
    [InlineData(FacilityState.Active, "state=ACTIVE")]
    [InlineData(FacilityState.Inactive, "state=INACTIVE")]
    [InlineData(FacilityState.Unknown, "state=UNKNOWN")]
    public async Task GetFacilitiesAsync_Should_FormatState_When_StateProvided(FacilityState state, string expected) {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery().WithState(state), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Theory]
    [InlineData("10562421", "equipmentnumbers=10562421")]
    [InlineData("10562421&22", "equipmentnumbers=10562421%2622")]
    public async Task GetFacilitiesAsync_Should_EncodeEquipmentNumbers_When_ValuesProvided(string number, string expected) {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery().WithEquipmentNumbers(number), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_JoinEquipmentNumbers_When_MultipleProvided() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery().WithEquipmentNumbers("1", "2"), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString()
            .ShouldContain("equipmentnumbers=1%2C2");
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_OmitEquipmentNumbers_When_NoneProvided() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldNotContain("equipmentnumbers");
    }

    [Theory]
    [InlineData(8.1, 50.2, 8.3, 50.4, true)]
    [InlineData(null, 50.2, 8.3, 50.4, false)]
    [InlineData(8.1, null, 8.3, 50.4, false)]
    [InlineData(8.1, 50.2, null, 50.4, false)]
    [InlineData(8.1, 50.2, 8.3, null, false)]
    public async Task GetFacilitiesAsync_Should_EmitArea_When_AllBoundsProvided(double? west, double? south, double? east, double? north, bool expected) {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        var query = new FacilityQuery {
            LongitudeWest = west,
            LatitudeSouth = south,
            LongitudeEast = east,
            LatitudeNorth = north
        };

        await fixture.Client.GetFacilitiesAsync(query, TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().Contains("area=").ShouldBe(expected);
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_FormatAreaInvariant_When_CultureUsesCommaDecimalSeparator() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");
        try {
            await fixture.Client.GetFacilitiesAsync(new FacilityQuery().WithArea(8.1, 50.2, 8.3, 50.4), TestContext.Current.CancellationToken);
        } finally {
            CultureInfo.CurrentCulture = original;
        }

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain("area=8.1%2C50.2%2C8.3%2C50.4");
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_SendNoQuery_When_QueryIsEmpty() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetFacilitiesAsync(new FacilityQuery(), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/facilities");
    }

    [Fact]
    public async Task GetFacilitiesAsync_Should_CombineAllFilters_When_MultipleFiltersProvided() {
        using var fixture = new FacilityClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        var query = new FacilityQuery()
            .WithType(FacilityType.Elevator)
            .WithState(FacilityState.Active)
            .WithEquipmentNumbers("10562421", "10562422")
            .AtStation(8002549)
            .WithArea(8.1, 50.2, 8.3, 50.4);

        await fixture.Client.GetFacilitiesAsync(query, TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/facilities?type=ELEVATOR&state=ACTIVE&equipmentnumbers=10562421%2C10562422&stationnumber=8002549&area=8.1%2C50.2%2C8.3%2C50.4");
    }
}
