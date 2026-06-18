using DotBahn.Facilities.Models;

namespace DotBahn.Facilities;

/// <summary>
/// Client for accessing the 'Deutsche Bahn FaSta'-API.
/// </summary>
public interface IFacilityClient {
    /// <summary>
    /// Finds facilities based on optional filter criteria.
    /// </summary>
    /// <param name="query">The query to specify results with.</param>
    /// <param name="cancellation">Token to cancel the request.</param>
    /// <returns>List of facilities matching the criteria.</returns>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur.</exception>
    Task<IReadOnlyList<Facility>> GetFacilitiesAsync(FacilityQuery query, CancellationToken cancellation = default);
}
