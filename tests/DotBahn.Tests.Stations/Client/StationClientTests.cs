using System.Net;
using DotBahn.Common.Parsing;
using DotBahn.Common.Transformer;
using DotBahn.Stations;
using DotBahn.Stations.Internal.Contracts;
using DotBahn.Stations.Models;
using DotBahn.Tests.Shared;
using Moq;

namespace DotBahn.Tests.Stations.Client;

public class StationClientTests : ClientTestBase {
    private readonly Mock<IParser<StationsResponseContract>> _parserMock = new();
    private readonly Mock<ITransformer<IEnumerable<Station>, StationsResponseContract>> _transformerMock = new();

    public StationClientTests() {
        HttpClient.BaseAddress = new Uri("https://api.deutschebahn.com");
    }

    [Fact]
    public async Task GetStationsAsync_BuildsStationsUrl() {
        var client = CreateClient();
        var query = new StationQuery();
        var contract = new StationsResponseContract();

        SetupMocks(contract);
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");

        await client.GetStationsAsync(query, TestContext.Current.CancellationToken);

        var requestUri = HttpHandler.SentRequests[0].RequestUri!.ToString();
        Assert.Contains("/stations", requestUri);
        var request = HttpHandler.SentRequests[0];

        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Contains("application/json", request.Headers.Accept.Select(h => h.MediaType));
    }

    [Fact]
    public async Task GetStationsAsync_AppliesQueryParametersToUrl() {
        var client = CreateClient();
        var query = new StationQuery { Names = ["Berlin"] };
        var contract = new StationsResponseContract();

        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(contract);
        _transformerMock.Setup(t => t.Transform(It.IsAny<StationsResponseContract>())).Returns([]);
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");

        await client.GetStationsAsync(query, TestContext.Current.CancellationToken);

        var request = HttpHandler.SentRequests[0];
        Assert.Contains("searchstring=Berlin", request.RequestUri?.ToString());
    }

    [Fact]
    public async Task GetStationsAsync_SortsStationsByCategoryAscending() {
        var client = CreateClient();
        var query = new StationQuery();
        var contract = new StationsResponseContract {
            Stations = [
                new StationContract { Number = 1, Name = "Station C", Category = 5 },
                new StationContract { Number = 2, Name = "Station A", Category = 2 },
                new StationContract { Number = 3, Name = "Station B", Category = 7 }
            ]
        };
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(contract);
        _transformerMock.Setup(t => t.Transform(It.IsAny<StationsResponseContract>())).Returns([]);
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");

        await client.GetStationsAsync(query, TestContext.Current.CancellationToken);

        Assert.Equal(2, contract.Stations[0].Category);
        Assert.Equal(5, contract.Stations[1].Category);
        Assert.Equal(7, contract.Stations[2].Category);
    }

    [Fact]
    public async Task GetStationsAsync_WithNullQuery_ThrowsArgumentNullException() {
        var client = CreateClient();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.GetStationsAsync(null!, TestContext.Current.CancellationToken));
    }

    private StationClient CreateClient() =>
        new(HttpClient, AuthorizationMock.Object, _parserMock.Object, _transformerMock.Object);

    private void SetupMocks(StationsResponseContract contract) {
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(contract);
        _transformerMock.Setup(t => t.Transform(It.IsAny<StationsResponseContract>())).Returns([]);
    }
}
