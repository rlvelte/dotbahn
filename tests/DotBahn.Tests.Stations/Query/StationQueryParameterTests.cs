using System.Net;
using DotBahn.Stations;
using DotBahn.Stations.Models.Enumerations;
using DotBahn.Tests.Stations.Support;
using Shouldly;

namespace DotBahn.Tests.Stations.Query;

public class StationQueryParameterTests {
    private const string EmptyResponse = """{"result":[]}""";

    [Theory]
    [InlineData("Hamburg", "searchstring=Hamburg%2A")]
    [InlineData("München", "searchstring=München%2A")]
    [InlineData("Ham*", "searchstring=Ham%2A")]
    [InlineData("H?mb", "searchstring=H%3Fmb")]
    public async Task GetStationsAsync_Should_EncodeNames_When_PatternsProvided(string name, string expected) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery().WithNames(name), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Theory]
    [InlineData("1", "category=1")]
    [InlineData("1,3-5", "category=1%2C3-5")]
    [InlineData("1 - 7", "category=1-7")]
    public async Task GetStationsAsync_Should_EncodeCategories_When_CategoriesProvided(string categories, string expected) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery().WithCategories(categories), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Theory]
    [InlineData(FederalState.Berlin, "federalstate=Berlin")]
    [InlineData(FederalState.BadenWuerttemberg, "federalstate=Baden-W%5Cu00FCrttemberg")]
    [InlineData(FederalState.NorthRhineWestphalia, "federalstate=Nordrhein-Westfalen")]
    [InlineData(FederalState.Unknown, "federalstate=Unknown")]
    public async Task GetStationsAsync_Should_FormatFederalState_When_StateProvided(FederalState state, string expected) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery().InFederalState(state), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Theory]
    [InlineData(LogicalOperator.And, "logicaloperator=AND")]
    [InlineData(LogicalOperator.Or, "logicaloperator=OR")]
    public async Task GetStationsAsync_Should_FormatLogicalOperator_When_OperatorProvided(LogicalOperator @operator, string expected) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery().CombineAs(@operator), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Theory]
    [InlineData(0, 10000, "offset=0&limit=10000")]
    [InlineData(1, 1, "offset=1&limit=1")]
    [InlineData(-1, -1, "offset=-1&limit=-1")]
    [InlineData(int.MaxValue, int.MaxValue, "offset=2147483647&limit=2147483647")]
    public async Task GetStationsAsync_Should_IncludePagination_When_BoundaryValuesProvided(int offset, int limit, string expected) {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery().Skip(offset).LimitTo(limit), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldContain(expected);
    }

    [Fact]
    public async Task GetStationsAsync_Should_SendOnlyDefaultParameters_When_QueryIsEmpty() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);

        await fixture.Client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/stations?logicaloperator=AND&offset=0&limit=10000");
    }

    [Fact]
    public async Task GetStationsAsync_Should_CombineAllFilters_When_MultipleFiltersProvided() {
        using var fixture = new StationClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, EmptyResponse);
        var query = new StationQuery()
            .WithNames("Hamburg")
            .WithCategories("1,3-5")
            .InFederalState(FederalState.Hamburg)
            .WithEva("8002549")
            .WithRil("AH")
            .CombineAs(LogicalOperator.Or)
            .Skip(10)
            .LimitTo(50);

        await fixture.Client.GetStationsAsync(query, TestContext.Current.CancellationToken);

        fixture.Handler.SentRequests.ShouldHaveSingleItem().RequestUri!.ToString().ShouldBe("https://api.example.com/stations?searchstring=Hamburg%2A&category=1%2C3-5&federalstate=Hamburg&eva=8002549&ril=AH&logicaloperator=OR&offset=10&limit=50");
    }
}
