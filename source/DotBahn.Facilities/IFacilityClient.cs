using DotBahn.Facilities.Models;

namespace DotBahn.Facilities;

/// <summary>
/// Client for accessing the 'Deutsche Bahn FaSta'-API
/// </summary>
public interface IFacilityClient {
    /// <summary>
    /// Finds facilities based on optional filter criteria
    /// </summary>
    /// <param name="query">The query to specify results with</param>
    /// <param name="ct">Token to cancel the request</param>
    /// <returns>List of facilities matching the criteria</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c></exception>
    /// <exception cref="HttpRequestException">Thrown when non-success status codes occur</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled</exception>
    Task<IReadOnlyList<Facility>> GetFacilitiesAsync(FacilityQuery query, CancellationToken ct = default);
}
