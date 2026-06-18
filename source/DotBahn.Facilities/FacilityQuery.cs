using DotBahn.Common.Utilities;
using DotBahn.Facilities.Models.Enumerations;
using FacilitiesJsonContext = DotBahn.Facilities.Internal.Json.FacilitiesJsonContext;

namespace DotBahn.Facilities;

/// <summary>
/// Represents the query parameters for searching facilities in stations.
/// Provides fluent methods for convenient building of queries.
/// </summary>
public sealed record FacilityQuery {
    /// <summary>
    /// Type of the facility.
    /// </summary>
    public FacilityType? Type { get; set; }

    /// <summary>
    /// State of the facility.
    /// </summary>
    public FacilityState? State { get; set; }

    /// <summary>
    /// Equipment numbers to filter by.
    /// </summary>
    public IEnumerable<string> EquipmentNumbers { get; set; } = [];

    /// <summary>
    /// The station ID to filter facilities.
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
    internal QueryParameters ToQueryParameters() => QueryParameters.Create()
        .Add("type", EnumUtil.Format(Type, FacilitiesJsonContext.Default.FacilityType))
        .Add("state", EnumUtil.Format(State, FacilitiesJsonContext.Default.FacilityState))
        .Add("equipmentnumbers", EquipmentNumbers)
        .Add("stationnumber", StationId);
}
