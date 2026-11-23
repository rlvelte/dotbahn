namespace DotBahn.Boards.Tests.Query;

public class BoardQueryFluentApiTests {
    [Fact]
    public void FluentApi_CompleteChaining_SetsAllProperties() {
        var start = new DateTime(2026, 11, 23, 10, 0, 0);
        var end = new DateTime(2026, 11, 23, 12, 0, 0);

        var query = new BoardQuery()
                    .ForEvaNumbers("8000105", "8000096")
                    .OnPlatforms("2", "3")
                    .WithBoardConfigurations("EXPANDED")
                    .StartingAt(start)
                    .EndingAt(end)
                    .WithTransportTypes("HIGH_SPEED", "REGIONAL")
                    .InLanguages("deu")
                    .WithMaxEntriesPerTransportType(50)
                    .WithMaxViaStops(5)
                    .WithLineColours()
                    .WithPlatformAccessibility()
                    .WithoutCancellations()
                    .WithOnlyReplacementTransports()
                    .SortByTimeSchedule();

        Assert.Equal(["8000105", "8000096"], query.EvaNumbers);
        Assert.Equal(["2", "3"], query.Platforms);
        Assert.Equal(["EXPANDED"], query.BoardConfigurations);
        Assert.Equal(start, query.Start);
        Assert.Equal(end, query.End);
        Assert.Equal(["HIGH_SPEED", "REGIONAL"], query.TransportTypes);
        Assert.Equal(["deu"], query.Languages);
        Assert.Equal(50, query.MaxEntriesPerTransportType);
        Assert.Equal(5, query.MaxViaStops);
        Assert.True(query.IncludeLineColours);
        Assert.True(query.IncludePlatformAccessibility);
        Assert.True(query.NoCancellations);
        Assert.True(query.OnlyReplacementTransports);
        Assert.Equal("TIME_SCHEDULE", query.SortBy);
    }

    [Fact]
    public void FluentApi_PartialChaining_KeepsDefaultsForUnspecifiedProperties() {
        var query = new BoardQuery()
                    .ForEvaNumbers("8000105")
                    .StartingAt(new DateTime(2026, 11, 23, 10, 0, 0));

        Assert.Equal(["8000105"], query.EvaNumbers);
        Assert.Null(query.StadaId);
        Assert.Null(query.Dhids);
        Assert.Null(query.Platforms);
        Assert.Null(query.BoardConfigurations);
        Assert.Null(query.End);
        Assert.Equal([], query.TransportTypes);
        Assert.Equal(["deu", "eng"], query.Languages);
        Assert.Equal(25, query.MaxEntriesPerTransportType);
        Assert.Null(query.MaxViaStops);
        Assert.False(query.IncludeLineColours);
        Assert.False(query.IncludePlatformAccessibility);
        Assert.False(query.NoCancellations);
        Assert.False(query.OnlyReplacementTransports);
        Assert.Equal("TIME", query.SortBy);
    }

    [Fact]
    public void FluentApi_Selectors_SetMutuallyExclusiveStations() {
        var byStada = new BoardQuery().ForStada("8000105");
        var byDhids = new BoardQuery().ForDhids("de:03241:8000105");
        var byCount = new BoardQuery().ForNext(10);

        Assert.Equal("8000105", byStada.StadaId);
        Assert.Equal(["de:03241:8000105"], byDhids.Dhids);
        Assert.Equal(10, byCount.EntryCount);
    }

    [Fact]
    public void FluentApi_Chaining_ReturnsNewImmutableInstances() {
        var query = new BoardQuery();

        var result1 = query.ForEvaNumbers("8000105");
        var result2 = result1.StartingAt(new DateTime(2026, 11, 23, 10, 0, 0));

        Assert.NotSame(query, result1);
        Assert.NotSame(result1, result2);
        Assert.Null(query.EvaNumbers);
        Assert.Null(result1.Start);
        Assert.Equal(["8000105"], result1.EvaNumbers);
        Assert.Equal(new DateTime(2026, 11, 23, 10, 0, 0), result2.Start);
    }

    [Fact]
    public void FluentApi_MixedWithObjectInitializer_ShouldCombineBothApproaches() {
        var query = new BoardQuery {
            MaxEntriesPerTransportType = 40
        }.ForEvaNumbers("8000105")
         .WithPlatformAccessibility();

        Assert.Equal(40, query.MaxEntriesPerTransportType);
        Assert.Equal(["8000105"], query.EvaNumbers);
        Assert.True(query.IncludePlatformAccessibility);
    }

    [Fact]
    public void FluentApi_OverwritingValues_UsesLastSetValue() {
        var query = new BoardQuery()
                    .ForEvaNumbers("8000105")
                    .ForEvaNumbers("8000096")
                    .SortByTimeSchedule()
                    .SortByTime();

        Assert.Equal(["8000096"], query.EvaNumbers);
        Assert.Equal("TIME", query.SortBy);
    }
}
