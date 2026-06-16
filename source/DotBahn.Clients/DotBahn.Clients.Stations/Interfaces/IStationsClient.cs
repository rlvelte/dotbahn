using DotBahn.Clients.Stations.Query;
using DotBahn.Data.Stations.Models;

namespace DotBahn.Clients.Stations.Interfaces;

/// <summary>
/// Client for accessing the 'Deutsche Bahn StaDa'-API.
/// </summary>
public interface IStationsClient {
    /// <summary>
    /// Searches for stations using a query structure.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <param name="cancellation">Token to cancel the request.</param>
    /// <returns>List of stations matching the search criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    Task<IReadOnlyList<Station>> GetStationsAsync(StationsQuery query, CancellationToken cancellation = default);
}
