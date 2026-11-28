using System.Net;
using DotBahn.Boards;
using DotBahn.Tests.Boards.Support;
using Shouldly;

namespace DotBahn.Tests.Boards.Query;

public class BoardQueryMappingTests {
    [Theory]
    [InlineData("\"maxViaStops\":3")]
    [InlineData("\"languages\":[\"deu\",\"eng\"]")]
    [InlineData("\"transportTypes\":null")]
    [InlineData("\"timeWindow\":null")]
    [InlineData("\"sortBy\":\"TIME\"")]
    [InlineData("\"maxEntriesPerTransportType\":25")]
    public async Task GetArrivalsAsync_Should_UseDocumentedDefaults_When_QueryIsEmpty(string expectedFragment) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");

        await fixture.Client.GetArrivalsAsync(new BoardQuery(), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequestBodies.ShouldHaveSingleItem().ShouldContain(expectedFragment);
    }

    [Theory]
    [InlineData(false, false, 0, "\"timeWindow\":null")]
    [InlineData(true, false, 0, "\"timeStart\":\"2024-12-31T12:00:00Z\"")]
    [InlineData(false, true, 0, "\"timeEnd\":\"2024-12-31T14:00:00Z\"")]
    [InlineData(false, false, 5, "\"entryCount\":5")]
    [InlineData(true, true, 5, "\"entryCount\":5")]
    public async Task GetArrivalsAsync_Should_MapTimeWindow_When_ValuesProvided(bool withStart, bool withEnd, int entryCount, string expectedFragment) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");
        var query = new BoardQuery();
        if (withStart) {
            query = query.StartingAt(new DateTime(2024, 12, 31, 12, 0, 0, DateTimeKind.Utc));
        }
        if (withEnd) {
            query = query.EndingAt(new DateTime(2024, 12, 31, 14, 0, 0, DateTimeKind.Utc));
        }
        if (entryCount > 0) {
            query = query.ForNext(entryCount);
        }

        await fixture.Client.GetArrivalsAsync(query, TestContext.Current.CancellationToken);

        fixture.Handler.SentRequestBodies.ShouldHaveSingleItem().ShouldContain(expectedFragment);
    }

    [Theory]
    [InlineData("\"evaNumbers\":null")]
    [InlineData("\"dhids\":null")]
    [InlineData("\"stada\":null")]
    [InlineData("\"platforms\":null")]
    [InlineData("\"boardConfigurations\":null")]
    public async Task GetDeparturesAsync_Should_MapStopPlaceSelector_When_QueryIsEmpty(string expectedFragment) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");

        await fixture.Client.GetDeparturesAsync(new BoardQuery(), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequestBodies.ShouldHaveSingleItem().ShouldContain(expectedFragment);
    }

    [Fact]
    public async Task GetDeparturesAsync_Should_MapAllSelectors_When_Provided() {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, "{\"meta\":{}}");
        var query = new BoardQuery()
            .ForEvaNumbers("8002549")
            .ForStada("8002549")
            .ForDhids("DH1")
            .OnPlatforms("1")
            .WithBoardConfigurations("TO_NEARBY")
            .WithTransportTypes("ICE")
            .InLanguages("deu")
            .WithMaxEntriesPerTransportType(10)
            .WithMaxViaStops(5)
            .WithLineColours()
            .WithPlatformAccessibility()
            .WithoutCancellations()
            .WithOnlyReplacementTransports()
            .SortByTimeSchedule();

        await fixture.Client.GetDeparturesAsync(query, TestContext.Current.CancellationToken);

        var body = fixture.Handler.SentRequestBodies.ShouldHaveSingleItem();
        body.ShouldContain("\"evaNumbers\":[\"8002549\"]");
        body.ShouldContain("\"stada\":\"8002549\"");
        body.ShouldContain("\"dhids\":[\"DH1\"]");
        body.ShouldContain("\"platforms\":[\"1\"]");
        body.ShouldContain("\"boardConfigurations\":[\"TO_NEARBY\"]");
        body.ShouldContain("\"transportTypes\":[\"ICE\"]");
        body.ShouldContain("\"languages\":[\"deu\"]");
        body.ShouldContain("\"maxEntriesPerTransportType\":10");
        body.ShouldContain("\"maxViaStops\":5");
        body.ShouldContain("\"includeLineColours\":true");
        body.ShouldContain("\"includePlatformAccessibility\":true");
        body.ShouldContain("\"noCancellations\":true");
        body.ShouldContain("\"onlyReplacementTransports\":true");
        body.ShouldContain("\"sortBy\":\"TIME_SCHEDULE\"");
    }

    [Theory]
    [InlineData("\"evaNumbers\":null")]
    [InlineData("\"dhids\":null")]
    [InlineData("\"platforms\":null")]
    [InlineData("\"boardConfigurations\":null")]
    public async Task GetArrivalsAsync_Should_MapEmptySelectorsAsNull_When_EmptyArraysProvided(string expectedFragment) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"meta":{}}""");
        var query = new BoardQuery()
            .ForEvaNumbers()
            .ForDhids()
            .OnPlatforms()
            .WithBoardConfigurations();

        await fixture.Client.GetArrivalsAsync(query, TestContext.Current.CancellationToken);

        fixture.Handler.SentRequestBodies.ShouldHaveSingleItem().ShouldContain(expectedFragment);
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_MapEmptyTransportTypesAndLanguagesAsNull_When_EmptyArraysProvided() {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"meta":{}}""");
        var query = new BoardQuery().WithTransportTypes().InLanguages();

        await fixture.Client.GetArrivalsAsync(query, TestContext.Current.CancellationToken);

        var body = fixture.Handler.SentRequestBodies.ShouldHaveSingleItem();
        body.ShouldContain("\"transportTypes\":null");
        body.ShouldContain("\"languages\":null");
    }

    [Fact]
    public async Task GetArrivalsAsync_Should_UseExplicitMaxViaStops_When_Provided() {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"meta":{}}""");

        await fixture.Client.GetArrivalsAsync(new BoardQuery().WithMaxViaStops(7), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequestBodies.ShouldHaveSingleItem().ShouldContain("\"maxViaStops\":7");
    }

    [Theory]
    [InlineData(DateTimeKind.Unspecified)]
    [InlineData(DateTimeKind.Local)]
    public async Task GetArrivalsAsync_Should_NormalizeStartTimeToUtc_When_KindProvided(DateTimeKind kind) {
        using var fixture = new BoardsClientFixture();
        fixture.Handler.RespondWith(HttpStatusCode.OK, """{"meta":{}}""");
        var local = new DateTime(2024, 12, 31, 12, 0, 0, DateTimeKind.Local);
        var start = kind == DateTimeKind.Local ? local : DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        var expected = (kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(start, DateTimeKind.Utc) : start.ToUniversalTime()).ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

        await fixture.Client.GetArrivalsAsync(new BoardQuery().StartingAt(start), TestContext.Current.CancellationToken);

        fixture.Handler.SentRequestBodies.ShouldHaveSingleItem().ShouldContain($"\"timeStart\":\"{expected}\"");
    }
}
