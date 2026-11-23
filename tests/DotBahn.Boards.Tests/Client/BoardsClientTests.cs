using System.Net;
using System.Text;
using DotBahn.Boards.Internal.Contracts;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Common.Parsing;
using DotBahn.Common.Tests;
using DotBahn.Common.Transformer;
using Moq;

namespace DotBahn.Boards.Tests.Client;

public class BoardsClientTests : ClientTestBase {
    private const string TestEva = "8000105";

    private readonly Mock<IParser<ArrivalBoardResponseContract>> _arrivalParserMock = new();
    private readonly Mock<IParser<DepartureBoardResponseContract>> _departureParserMock = new();
    private readonly Mock<ITransformer<Board<ArrivalEntry>, ArrivalBoardResponseContract>> _arrivalTransformerMock = new();
    private readonly Mock<ITransformer<Board<DepartureEntry>, DepartureBoardResponseContract>> _departureTransformerMock = new();

    private readonly ArrivalBoardResponseContract _arrivalContract = new();
    private readonly DepartureBoardResponseContract _departureContract = new();
    private readonly Board<ArrivalEntry> _arrivalBoard = new();
    private readonly Board<DepartureEntry> _departureBoard = new();

    public BoardsClientTests() {
        HttpClient.BaseAddress = new Uri("https://api.deutschebahn.com");
        _arrivalParserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(_arrivalContract);
        _departureParserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(_departureContract);
        _arrivalTransformerMock.Setup(t => t.Transform(It.IsAny<ArrivalBoardResponseContract>())).Returns(_arrivalBoard);
        _departureTransformerMock.Setup(t => t.Transform(It.IsAny<DepartureBoardResponseContract>())).Returns(_departureBoard);
    }

    [Fact]
    public async Task GetDeparturesAsync_PostsJsonToDeparturesEndpoint() {
        var client = CreateClient();
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");

        var result = await client.GetDeparturesAsync(new BoardQuery().ForEvaNumbers(TestEva), TestContext.Current.CancellationToken);

        Assert.Same(_departureBoard, result);
        AssertRequest(HttpMethod.Post, "/departures", "application/json");
        Assert.Equal("application/json", HttpHandler.SentRequests[0].Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetArrivalsAsync_PostsJsonToArrivalsEndpoint() {
        var client = CreateClient();
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");

        var result = await client.GetArrivalsAsync(new BoardQuery().ForEvaNumbers(TestEva), TestContext.Current.CancellationToken);

        Assert.Same(_arrivalBoard, result);
        AssertRequest(HttpMethod.Post, "/arrivals", "application/json");
        Assert.Equal("application/json", HttpHandler.SentRequests[0].Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetDeparturesAsync_SerializesQueryAsCamelCaseJsonBody() {
        var client = CreateClient();
        string? body = null;
        HttpHandler.RespondWith(request => {
            body = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        });
        var query = new BoardQuery()
                    .ForEvaNumbers(TestEva)
                    .StartingAt(new DateTime(2026, 11, 23, 9, 0, 0, DateTimeKind.Unspecified))
                    .EndingAt(new DateTime(2026, 11, 23, 11, 0, 0, DateTimeKind.Unspecified))
                    .WithPlatformAccessibility();

        await client.GetDeparturesAsync(query, TestContext.Current.CancellationToken);

        Assert.NotNull(body);
        Assert.Contains("\"evaNumbers\":[\"" + TestEva + "\"]", body);
        Assert.Contains("\"timeStart\":\"2026-11-23T09:00:00Z\"", body);
        Assert.Contains("\"timeEnd\":\"2026-11-23T11:00:00Z\"", body);
        Assert.Contains("\"includePlatformAccessibility\":true", body);
    }

    [Fact]
    public async Task GetArrivalsAsync_ParsesAndTransformsResponse() {
        var client = CreateClient();
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");

        await client.GetArrivalsAsync(new BoardQuery().ForEvaNumbers(TestEva), TestContext.Current.CancellationToken);

        _arrivalParserMock.Verify(p => p.Parse("{}"), Times.Once);
        _arrivalTransformerMock.Verify(t => t.Transform(_arrivalContract), Times.Once);
    }

    [Fact]
    public async Task GetArrivalsAsync_WithNullQuery_ThrowsArgumentNullException() {
        var client = CreateClient();

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetArrivalsAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetDeparturesAsync_WithNullQuery_ThrowsArgumentNullException() {
        var client = CreateClient();

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetDeparturesAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetArrivalsAsync_CancellationRequested_ThrowsOperationCanceledException() {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        HttpHandler.RespondWith(_ => throw new OperationCanceledException(cts.Token));
        var client = CreateClient();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.GetArrivalsAsync(new BoardQuery().ForEvaNumbers(TestEva), cts.Token));
    }

    [Fact]
    public async Task GetDeparturesAsync_CancellationRequested_ThrowsOperationCanceledException() {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        HttpHandler.RespondWith(_ => throw new OperationCanceledException(cts.Token));
        var client = CreateClient();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.GetDeparturesAsync(new BoardQuery().ForEvaNumbers(TestEva), cts.Token));
    }

    private BoardsClient CreateClient() => new(HttpClient, AuthorizationMock.Object, _arrivalParserMock.Object, _departureParserMock.Object, _arrivalTransformerMock.Object, _departureTransformerMock.Object);
}
