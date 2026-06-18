using System.ComponentModel;
using DotBahn.Facilities.Enumerations;
using DotBahn.Shared;
using DotBahn.Shared.Enumerations;
using FacilitiesJsonContext = DotBahn.Facilities.Json.FacilitiesJsonContext;

namespace DotBahn.Facilities;

/// <summary>
/// Represents the query parameters for searching facilities in stations.
/// Provides fluent methods for convenient building of queries.
/// </summary>
public sealed record FacilityQuery {
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
    public IEnumerable<string> EquipmentNumbers { get; set; } = [];

    /// <summary>
    /// The station ID to filter facilities. Optional filter.
    /// </summary>
    public string? StationId { get; set; }

    /// <summary>
    /// Sets the facility type filter.
    /// </summary>
    public FacilityQuery WithType(FacilityType type) {
        Type = type;
        return this;
    }

    /// <summary>
    /// Sets the facility state filter.
    /// </summary>
    public FacilityQuery WithState(FacilityState state) {
        State = state;
        return this;
    }

    /// <summary>
    /// Filters facilities by one or more equipment numbers.
    /// </summary>
    public FacilityQuery WithEquipmentNumbers(params string[] numbers) {
        EquipmentNumbers = numbers;
        return this;
    }

    /// <summary>
    /// Filters facilities by station ID.
    /// </summary>
    public FacilityQuery AtStation(int stationId) {
        StationId = stationId.ToString();
        return this;
    }

    /// <summary>
    /// Converts the query into <see cref="QueryParameters"/> for API calls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public QueryParameters ToQueryParameters() => QueryParameters.Create()
        .Add("type", EnumMapper.Format(Type, FacilitiesJsonContext.Default.FacilityType))
        .Add("state", EnumMapper.Format(State, FacilitiesJsonContext.Default.FacilityState))
        .Add("equipmentnumbers", EquipmentNumbers)
        .Add("stationnumber", StationId);
}
