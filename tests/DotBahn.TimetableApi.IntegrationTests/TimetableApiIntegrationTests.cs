using DotBahn.Core.Client;
using DotBahn.Core.Token;
using DotBahn.TimetableApi.Client;
using FluentAssertions;

namespace DotBahn.TimetableApi.IntegrationTests;

public class TimetableApiIntegrationTests : IDisposable {
    private readonly TimetableApiClient _client;
    private readonly HttpClient _httpClient;

    public TimetableApiIntegrationTests() {
        var config = new BaseClientConfiguration {
            BaseUrl = "https://api.deutschebahn.com/timetables/v1",
            ClientId = "ABC" ?? throw new Exception("CLIENT_ID not found."),
            ClientSecret = "ABC" ?? throw new Exception("CLIENT_KEY not found.")
        };

        _httpClient = new HttpClient();
        var tokenService = new TokenService(config, _httpClient);
        _client = new TimetableApiClient(config, tokenService, _httpClient);
    }

    [Fact]
    public async Task SearchStationsAsync_ShouldReturnStations_WhenPatternMatches() {
        // Act
        var stations = await _client.SearchStationsAsync("Frankfurt");

        // Assert
        stations.Should().NotBeEmpty();
        stations.Any(s => s.Name.Contains("Frankfurt")).Should().BeTrue();
        stations.Any(s => !string.IsNullOrEmpty(s.Eva)).Should().BeTrue();
    }

    [Fact]
    public async Task GetFullChangesAsync_ShouldReturnTimetable_ForValidEva() {
        // Arrange
        // Frankfurt Hbf EVA
        const string frankfurtEva = "8000105";

        // Act
        var timetable = await _client.GetFullChangesAsync(frankfurtEva);

        // Assert
        timetable.Should().NotBeNull();
        timetable.Station.Should().NotBeNullOrEmpty();
        
        // Fchg might be empty depending on time, but if it has stops, they should be valid
        if (timetable.Stops.Count > 0) {
            timetable.Stops.All(s => s.Station.Eva == frankfurtEva).Should().BeTrue();
        }
    }
    
    public void Dispose() {
        _httpClient.Dispose();
        _client.Dispose();
    }
}
