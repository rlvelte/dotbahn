using DotBahn.Boards;
using Shouldly;

namespace DotBahn.Tests.Boards.Query;

public class BoardQueryTests {
    [Fact]
    public void Constructor_Should_UseDocumentedDefaults_When_Called() {
        var sut = new BoardQuery();

        sut.EvaNumbers.ShouldBeNull();
        sut.StadaId.ShouldBeNull();
        sut.Dhids.ShouldBeNull();
        sut.Platforms.ShouldBeNull();
        sut.BoardConfigurations.ShouldBeNull();
        sut.Start.ShouldBeNull();
        sut.End.ShouldBeNull();
        sut.EntryCount.ShouldBeNull();
        sut.TransportTypes.ShouldBeEmpty();
        sut.Languages.ShouldBe(["deu", "eng"]);
        sut.MaxEntriesPerTransportType.ShouldBe(25);
        sut.MaxViaStops.ShouldBeNull();
        sut.IncludeLineColours.ShouldBeFalse();
        sut.IncludePlatformAccessibility.ShouldBeFalse();
        sut.NoCancellations.ShouldBeFalse();
        sut.OnlyReplacementTransports.ShouldBeFalse();
        sut.SortBy.ShouldBe("TIME");
    }

    [Theory]
    [InlineData("8002549")]
    [InlineData("8002549", "8002548")]
    public void ForEvaNumbers_Should_ReturnNewQuery_When_ValuesProvided(params string[] numbers) {
        var sut = new BoardQuery();

        var result = sut.ForEvaNumbers(numbers);

        result.ShouldNotBeSameAs(sut);
        result.EvaNumbers.ShouldBe(numbers);
        sut.EvaNumbers.ShouldBeNull();
    }

    [Theory]
    [InlineData("8002549")]
    [InlineData("8002549", "8002548")]
    public void ForDhids_Should_ReturnNewQuery_When_ValuesProvided(params string[] dhids) {
        var sut = new BoardQuery();

        var result = sut.ForDhids(dhids);

        result.ShouldNotBeSameAs(sut);
        result.Dhids.ShouldBe(dhids);
        sut.Dhids.ShouldBeNull();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1", "2")]
    public void OnPlatforms_Should_ReturnNewQuery_When_ValuesProvided(params string[] platforms) {
        var sut = new BoardQuery();

        var result = sut.OnPlatforms(platforms);

        result.ShouldNotBeSameAs(sut);
        result.Platforms.ShouldBe(platforms);
        sut.Platforms.ShouldBeNull();
    }

    [Theory]
    [InlineData("TO_NEARBY")]
    [InlineData("TO_NEARBY", "ALL")]
    public void WithBoardConfigurations_Should_ReturnNewQuery_When_ValuesProvided(params string[] configurations) {
        var sut = new BoardQuery();

        var result = sut.WithBoardConfigurations(configurations);

        result.ShouldNotBeSameAs(sut);
        result.BoardConfigurations.ShouldBe(configurations);
        sut.BoardConfigurations.ShouldBeNull();
    }

    [Theory]
    [InlineData("ICE")]
    [InlineData("ICE", "IC")]
    public void WithTransportTypes_Should_ReturnNewQuery_When_ValuesProvided(params string[] types) {
        var sut = new BoardQuery();

        var result = sut.WithTransportTypes(types);

        result.ShouldNotBeSameAs(sut);
        result.TransportTypes.ShouldBe(types);
        sut.TransportTypes.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("deu")]
    [InlineData("deu", "eng")]
    public void InLanguages_Should_ReturnNewQuery_When_ValuesProvided(params string[] languages) {
        var sut = new BoardQuery();

        var result = sut.InLanguages(languages);

        result.ShouldNotBeSameAs(sut);
        result.Languages.ShouldBe(languages);
        sut.Languages.ShouldBe(["deu", "eng"]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("8002549")]
    public void ForStada_Should_SetStadaId_When_ValueProvided(string stadaId) {
        var sut = new BoardQuery();

        var result = sut.ForStada(stadaId);

        result.ShouldNotBeSameAs(sut);
        result.StadaId.ShouldBe(stadaId);
        sut.StadaId.ShouldBeNull();
    }

    [Fact]
    public void StartingAt_And_EndingAt_Should_SetTimeWindow_When_ValuesProvided() {
        var start = new DateTime(2024, 12, 31, 12, 0, 0);
        var end = start.AddHours(2);
        var sut = new BoardQuery();

        var result = sut.StartingAt(start).EndingAt(end);

        result.Start.ShouldBe(start);
        result.End.ShouldBe(end);
        sut.Start.ShouldBeNull();
        sut.End.ShouldBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void ForNext_Should_SetEntryCount_When_ValueProvided(int count) {
        var sut = new BoardQuery();

        var result = sut.ForNext(count);

        result.EntryCount.ShouldBe(count);
        sut.EntryCount.ShouldBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(int.MaxValue)]
    public void WithMaxEntriesPerTransportType_Should_SetLimit_When_ValueProvided(int max) {
        var sut = new BoardQuery();

        var result = sut.WithMaxEntriesPerTransportType(max);

        result.MaxEntriesPerTransportType.ShouldBe(max);
        sut.MaxEntriesPerTransportType.ShouldBe(25);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(int.MaxValue)]
    public void WithMaxViaStops_Should_SetLimit_When_ValueProvided(int max) {
        var sut = new BoardQuery();

        var result = sut.WithMaxViaStops(max);

        result.MaxViaStops.ShouldBe(max);
        sut.MaxViaStops.ShouldBeNull();
    }

    [Fact]
    public void FlagAndSortMethods_Should_SetValues_When_Called() {
        var result = new BoardQuery()
            .WithLineColours()
            .WithPlatformAccessibility()
            .WithoutCancellations()
            .WithOnlyReplacementTransports()
            .SortByTimeSchedule();

        result.IncludeLineColours.ShouldBeTrue();
        result.IncludePlatformAccessibility.ShouldBeTrue();
        result.NoCancellations.ShouldBeTrue();
        result.OnlyReplacementTransports.ShouldBeTrue();
        result.SortBy.ShouldBe("TIME_SCHEDULE");

        result.SortByTime().SortBy.ShouldBe("TIME");
    }
}
