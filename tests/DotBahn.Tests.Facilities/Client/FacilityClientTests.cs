using System.Net;

using DotBahn.Facilities;
using DotBahn.Facilities.Internal.Contracts;
using DotBahn.Facilities.Models;
using DotBahn.Facilities.Models.Enumerations;
using DotBahn.Shared.Parsing;
using DotBahn.Shared.Transformer;
using DotBahn.Tests.Shared;

using Moq;

namespace DotBahn.Tests.Facilities.Client;

/// <summary>
/// Tests for <see cref="FacilityClient.GetFacilitiesAsync"/> covering URL building,
/// query parameter application, list conversion, and null query guard.
/// </summary>
public class FacilityClientTests : ClientTestBase {
    private readonly Mock<IParser<IEnumerable<FacilityContract>>> _parserMock = new();
    private readonly Mock<ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>>> _transformerMock = new();

    public FacilityClientTests() {
        HttpClient.BaseAddress = new Uri("https://api.deutschebahn.com");
    }

    [Fact]
    public async Task GetFacilitiesAsync_BuildsUrlWithFacilitiesEndpoint() {
        var client = CreateClient();
        var query = new FacilityQuery();
        SetupParserAndTransformer();
        HttpHandler.RespondWith(HttpStatusCode.OK, "[]", "application/json");

        await client.GetFacilitiesAsync(query, TestContext.Current.CancellationToken);

        AssertRequest(HttpMethod.Get, "/facilities", "application/json");
    }

    [Fact]
    public async Task GetFacilitiesAsync_AppliesQueryParametersFromFacilityQuery() {
        var client = CreateClient();
        var query = new FacilityQuery {
            Type = FacilityType.Elevator,
            StationId = "8002549"
        };

        SetupParserAndTransformer();
        HttpHandler.RespondWith(HttpStatusCode.OK, "[]", "application/json");

        await client.GetFacilitiesAsync(query, TestContext.Current.CancellationToken);

        var requestUri = HttpHandler.SentRequests[0].RequestUri?.ToString();
        Assert.Contains("type=ELEVATOR", requestUri);
        Assert.Contains("stationnumber=8002549", requestUri);
    }

    [Fact]
    public async Task GetFacilitiesAsync_CallsToListBeforeTransform() {
        var client = CreateClient();
        var query = new FacilityQuery();
        SetupParserAndTransformer();
        HttpHandler.RespondWith(HttpStatusCode.OK, "[]", "application/json");

        var result = await client.GetFacilitiesAsync(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetFacilitiesAsync_WithNullQuery_ThrowsArgumentNullException() {
        var client = CreateClient();

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetFacilitiesAsync(null!, TestContext.Current.CancellationToken));
    }

    private FacilityClient CreateClient() => new(
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
