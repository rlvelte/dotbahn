using DotBahn.Clients.Facilities.Query;
using DotBahn.Data.Facilities.Models;

namespace DotBahn.Clients.Facilities.Interfaces;

/// <summary>
/// Client for accessing the 'Deutsche Bahn FaSta'-API.
/// </summary>
public interface IFacilitiesClient {
    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <param name="cancellation">Token to cancel the request.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    Task<IReadOnlyList<Facility>> GetFacilitiesAsync(FacilitiesQuery query, CancellationToken cancellation = default);
}
