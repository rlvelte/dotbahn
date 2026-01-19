using System.ComponentModel;
using DotBahn.Clients.Shared.Query;
using DotBahn.Data.Facilities.Enumerations;
using DotBahn.Data.Shared.Enumerations;

namespace DotBahn.Clients.Facilities.Query;

/// <summary>
/// Represents the query parameters for searching facilities in stations.
/// Provides fluent methods for convenient building of queries.
/// </summary>
public sealed record FacilitiesQuery {
    /// <summary>
    /// Type of the facility. Optional filter.
    /// </summary>
    public FacilityType? Type { get; set; }

    /// <summary>
    /// State of the facility. Optional filter.
    /// </summary>
    public FacilityState? State { get; set; }

    /// <summary>
    /// Equipment numbers to filter by. Optional filter.
    /// </summary>
    public string[]? EquipmentNumbers { get; set; }

    /// <summary>
    /// The station ID to filter facilities. Optional filter.
    /// </summary>
    public string? StationId { get; set; }

    /// <summary>
    /// Sets the facility type filter.
    /// </summary>
    public FacilitiesQuery WithType(FacilityType type) {
        Type = type;
        return this;
    }

    /// <summary>
    /// Sets the facility state filter.
    /// </summary>
    public FacilitiesQuery WithState(FacilityState state) {
        State = state;
        return this;
    }

    /// <summary>
    /// Filters facilities by one or more equipment numbers.
    /// </summary>
    public FacilitiesQuery WithEquipmentNumbers(params string[] numbers) {
        EquipmentNumbers = numbers;
        return this;
    }

    /// <summary>
    /// Filters facilities by station ID.
    /// </summary>
    public FacilitiesQuery AtStation(int stationId) {
        StationId = stationId.ToString();
        return this;
    }

    /// <summary>
    /// Converts the query into <see cref="QueryParameters"/> for API calls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public QueryParameters ToQueryParameters() => QueryParameters.Create()
                                                                 .Add("type", Type?.GetAssociatedValue())
                                                                 .Add("state", State?.GetAssociatedValue())
                                                                 .Add("equipmentnumbers", EquipmentNumbers)
                                                                 .Add("stationnumber", StationId);
}
