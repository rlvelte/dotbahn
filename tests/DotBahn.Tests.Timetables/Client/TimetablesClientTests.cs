using System.Net;

using DotBahn.Clients.Timetables;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Timetables.Models;
using DotBahn.Modules.Shared.Parsing.Base;
using DotBahn.Tests.Shared;

using Moq;

namespace DotBahn.Tests.Timetables.Client;

public class TimetablesClientTests : ClientTestBase {
    private const int TestEva = 8000105;

    private readonly Mock<IParser<TimetableResponseContract>> _parserMock = new();
    private readonly Timetable _defaultTimetable = new() { Station = "TestStation" };
    private readonly TimetableResponseContract _defaultContract = new() { Station = "TestStation" };

    public TimetablesClientTests() {
        HttpClient.BaseAddress = new Uri("https://api.deutschebahn.com");
    }

    private sealed class StubTransformer : ITransformer<Timetable, TimetableResponseContract> {
        private readonly Timetable _result;
        public StubTransformer(Timetable result) => _result = result;
        public int TransformCallCount { get; private set; }
        public Timetable Transform(in TimetableResponseContract contract) {
            TransformCallCount++;
            return _result;
        }
    }

    private sealed class StubMerger : IMerger<Timetable> {
        private readonly Timetable _result;
        public StubMerger(Timetable result) => _result = result;
        public int MergeCallCount { get; private set; }
        public Timetable Merge(Timetable current, in Timetable changes) {
            MergeCallCount++;
            return _result;
        }
    }

    [Fact]
    public async Task GetFullChangesAsync_WithoutCurrentTimetable_ReturnsTransformedChanges() {
        var transformer = new StubTransformer(_defaultTimetable);
        var merger = new StubMerger(_defaultTimetable);
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(_defaultContract);
        HttpHandler.RespondWith(HttpStatusCode.OK, "<timetable station=\"TestStation\"/>");

        var client = new TimetablesClient(HttpClient, AuthorizationMock.Object,
            _parserMock.Object, transformer, merger, CacheMock.Object);
        var result = await client.GetFullChangesAsync(TestEva);

        Assert.Same(_defaultTimetable, result);
        Assert.Equal(1, transformer.TransformCallCount);
        Assert.Equal(0, merger.MergeCallCount);
        AssertRequest(HttpMethod.Get, "/fchg/8000105", "application/xml");
    }

    [Fact]
    public async Task GetFullChangesAsync_WithCurrentTimetable_CallsMergerMerge() {
        var currentTimetable = new Timetable { Station = "CurrentStation" };
        var changesTimetable = new Timetable { Station = "ChangesStation" };
        var mergedTimetable = new Timetable { Station = "MergedStation" };
        var transformer = new StubTransformer(changesTimetable);
        var merger = new StubMerger(mergedTimetable);
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(_defaultContract);
        HttpHandler.RespondWith(HttpStatusCode.OK, "<timetable station=\"ChangesStation\"/>");

        var client = new TimetablesClient(HttpClient, AuthorizationMock.Object,
            _parserMock.Object, transformer, merger, CacheMock.Object);
        var result = await client.GetFullChangesAsync(TestEva, currentTimetable);

        Assert.Same(mergedTimetable, result);
        Assert.Equal(1, transformer.TransformCallCount);
        Assert.Equal(1, merger.MergeCallCount);
        AssertRequest(HttpMethod.Get, "/fchg/8000105", "application/xml");
    }

    [Fact]
    public async Task GetRecentChangesAsync_WithoutCurrentTimetable_ReturnsTransformedChanges() {
        var transformer = new StubTransformer(_defaultTimetable);
        var merger = new StubMerger(_defaultTimetable);
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(_defaultContract);
        HttpHandler.RespondWith(HttpStatusCode.OK, "<timetable station=\"TestStation\"/>");

        var client = new TimetablesClient(HttpClient, AuthorizationMock.Object,
            _parserMock.Object, transformer, merger, CacheMock.Object);
        var result = await client.GetRecentChangesAsync(TestEva);

        Assert.Same(_defaultTimetable, result);
        Assert.Equal(1, transformer.TransformCallCount);
        Assert.Equal(0, merger.MergeCallCount);
        AssertRequest(HttpMethod.Get, "/rchg/8000105", "application/xml");
    }

    [Fact]
    public async Task GetRecentChangesAsync_WithCurrentTimetable_CallsMergerMerge() {
        var currentTimetable = new Timetable { Station = "CurrentStation" };
        var changesTimetable = new Timetable { Station = "RecentChangesStation" };
        var mergedTimetable = new Timetable { Station = "MergedRecentStation" };
        var transformer = new StubTransformer(changesTimetable);
        var merger = new StubMerger(mergedTimetable);
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(_defaultContract);
        HttpHandler.RespondWith(HttpStatusCode.OK, "<timetable station=\"RecentChangesStation\"/>");

        var client = new TimetablesClient(HttpClient, AuthorizationMock.Object,
            _parserMock.Object, transformer, merger, CacheMock.Object);
        var result = await client.GetRecentChangesAsync(TestEva, currentTimetable);

        Assert.Same(mergedTimetable, result);
        Assert.Equal(1, transformer.TransformCallCount);
        Assert.Equal(1, merger.MergeCallCount);
        AssertRequest(HttpMethod.Get, "/rchg/8000105", "application/xml");
    }

    [Fact]
    public async Task GetTimetableAsync_ValidDateTime_BuildsCorrectUrl() {
        var dateTime = new DateTime(2025, 6, 15, 14, 30, 0);
        var transformer = new StubTransformer(_defaultTimetable);
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(_defaultContract);
        HttpHandler.RespondWith(HttpStatusCode.OK, "<timetable station=\"TestStation\"/>");

        var client = new TimetablesClient(HttpClient, AuthorizationMock.Object,
            _parserMock.Object, transformer, new StubMerger(_defaultTimetable), CacheMock.Object);
        var result = await client.GetTimetableAsync(TestEva, dateTime);

        Assert.Same(_defaultTimetable, result);
        Assert.Equal(1, transformer.TransformCallCount);
        AssertRequest(HttpMethod.Get, "/plan/8000105/250615/14", "application/xml");
    }

    [Fact]
    public async Task GetFullChangesAsync_CancellationRequested_ThrowsOperationCanceledException() {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        HttpHandler.RespondWith(_ => throw new OperationCanceledException(cts.Token));

        var client = new TimetablesClient(HttpClient, AuthorizationMock.Object,
            _parserMock.Object, new StubTransformer(_defaultTimetable),
            new StubMerger(_defaultTimetable), CacheMock.Object);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            client.GetFullChangesAsync(TestEva, null, cts.Token));
    }

    private void AssertRequest(HttpMethod method, string relativeUrl, string acceptHeader) {
        var match = HttpHandler.SentRequests
            .Where(r => r.Method == method && r.RequestUri?.ToString().EndsWith(relativeUrl.TrimStart('/'), StringComparison.Ordinal) == true)
            .ToList();
        Assert.NotEmpty(match);
        Assert.Single(match);
        Assert.True(match[0].Headers.Accept.Any(h => h.MediaType == acceptHeader),
            $"Expected Accept header '{acceptHeader}' not found.");
    }
}
