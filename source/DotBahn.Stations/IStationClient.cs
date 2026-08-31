using DotBahn.Stations.Models;

namespace DotBahn.Stations;

/// <summary>
/// Client for accessing the 'Deutsche Bahn StaDa'-API
/// </summary>
public interface IStationClient {
    /// <summary>
    /// Searches for stations using a query structure
    /// </summary>
    /// <param name="query">The query to specify results with</param>
    /// <param name="cancellation">Token to cancel the request</param>
    /// <returns>List of stations matching the search criteria</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur</exception>
    Task<IReadOnlyList<Station>> GetStationsAsync(StationQuery query, CancellationToken cancellation = default);
}
