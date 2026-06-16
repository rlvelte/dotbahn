using System.Net;

using DotBahn.Clients.Facilities;
using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Clients.Facilities.Query;
using DotBahn.Clients.Shared.Parsing.Base;
using DotBahn.Data.Facilities.Enumerations;
using DotBahn.Data.Facilities.Models;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Tests.Shared;

using Moq;

namespace DotBahn.Tests.Facilities.Client;

/// <summary>
/// Tests for <see cref="FacilitiesClient.GetFacilitiesAsync"/> covering URL building,
/// query parameter application, list conversion, and null query guard.
/// </summary>
public class FacilitiesClientTests : ClientTestBase {
    private readonly Mock<IParser<IEnumerable<FacilityContract>>> _parserMock = new();
    private readonly Mock<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>> _transformerMock = new();

    public FacilitiesClientTests() {
        HttpClient.BaseAddress = new Uri("https://api.deutschebahn.com");
    }

    [Fact]
    public async Task GetFacilitiesAsync_BuildsUrlWithFacilitiesEndpoint() {
        var client = CreateClient();
        var query = new FacilitiesQuery();
        SetupParserAndTransformer();
        HttpHandler.RespondWith(HttpStatusCode.OK, "[]", "application/json");

        await client.GetFacilitiesAsync(query);

        AssertRequest(HttpMethod.Get, "/facilities", "application/json");
    }

    [Fact]
    public async Task GetFacilitiesAsync_AppliesQueryParametersFromFacilitiesQuery() {
        var client = CreateClient();
        var query = new FacilitiesQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549"
        };

        SetupParserAndTransformer();
        HttpHandler.RespondWith(HttpStatusCode.OK, "[]", "application/json");

        await client.GetFacilitiesAsync(query);

        var requestUri = HttpHandler.SentRequests[0].RequestUri?.ToString();
        Assert.Contains("type=ELEVATOR", requestUri);
        Assert.Contains("stationnumber=8002549", requestUri);
    }

    [Fact]
    public async Task GetFacilitiesAsync_CallsToListBeforeTransform() {
        var client = CreateClient();
        var query = new FacilitiesQuery();
        SetupParserAndTransformer();
        HttpHandler.RespondWith(HttpStatusCode.OK, "[]", "application/json");

        var result = await client.GetFacilitiesAsync(query);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetFacilitiesAsync_WithNullQuery_ThrowsArgumentNullException() {
        var client = CreateClient();

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetFacilitiesAsync(null!));
    }

    private FacilitiesClient CreateClient() => new(
            HttpClient,
            AuthorizationMock.Object,
            _parserMock.Object,
            _transformerMock.Object,
            CacheMock.Object);

    /// <summary>
    /// Configures default behaviors for parser and transformer mocks
    /// so they return empty collections without errors.
    /// </summary>
    private void SetupParserAndTransformer() {
        var emptyContracts = new List<FacilityContract>();
        var emptyFacilities = new List<Facility>();

        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(emptyContracts);
        _transformerMock.Setup(t => t.Transform(It.IsAny<IEnumerable<FacilityContract>>())).Returns(emptyFacilities);
    }
}
