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
    /// Westernmost longitude in WGS84 decimal degrees for geographic bounding box filter.
    /// <remarks>
    /// Must be used together with <see cref="LatitudeSouth"/>, <see cref="LongitudeEast"/>, and <see cref="LatitudeNorth"/>.
    /// </remarks>
    /// </summary>
    public double? LongitudeWest { get; set; }

    /// <summary>
    /// Southernmost latitude in WGS84 decimal degrees for geographic bounding box filter.
    /// <remarks>
    /// Must be used together with <see cref="LongitudeWest"/>, <see cref="LongitudeEast"/>, and <see cref="LatitudeNorth"/>.
    /// </remarks>
    /// </summary>
    public double? LatitudeSouth { get; set; }

    /// <summary>
    /// Easternmost longitude in WGS84 decimal degrees for geographic bounding box filter.
    /// <remarks>
    /// Must be used together with <see cref="LongitudeWest"/>, <see cref="LatitudeSouth"/>, and <see cref="LatitudeNorth"/>.
    /// </remarks>
    /// </summary>
    public double? LongitudeEast { get; set; }

    /// <summary>
    /// Northernmost latitude in WGS84 decimal degrees for geographic bounding box filter.
    /// <remarks>
    /// Must be used together with <see cref="LongitudeWest"/>, <see cref="LatitudeSouth"/>, and <see cref="LongitudeEast"/>.
    /// </remarks>
    /// </summary>
    public double? LatitudeNorth { get; set; }

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
    /// Filters facilities within a geographic bounding box.
    /// </summary>
    /// <param name="lngWest">Westernmost longitude in WGS84 decimal degrees.</param>
    /// <param name="latSouth">Southernmost latitude in WGS84 decimal degrees.</param>
    /// <param name="lngEast">Easternmost longitude in WGS84 decimal degrees.</param>
    /// <param name="latNorth">Northernmost latitude in WGS84 decimal degrees.</param>
    /// <returns>The current <see cref="FacilityQuery"/> instance for fluent chaining.</returns>
    public FacilityQuery WithArea(double lngWest, double latSouth, double lngEast, double latNorth) {
        LongitudeWest = lngWest;
        LatitudeSouth = latSouth;
        LongitudeEast = lngEast;
        LatitudeNorth = latNorth;
        return this;
    }

    /// <summary>
    /// Converts the query into <see cref="QueryParameters"/> for API calls.
    /// </summary>
    internal QueryParameters ToQueryParameters() {
        var area = LongitudeWest.HasValue && LatitudeSouth.HasValue && LongitudeEast.HasValue && LatitudeNorth.HasValue
            ? FormattableString.Invariant($"{LongitudeWest},{LatitudeSouth},{LongitudeEast},{LatitudeNorth}") : null;

        return QueryParameters.Create()
            .Add("type", EnumUtil.Format(Type, FacilitiesJsonContext.Default.FacilityType))
            .Add("state", EnumUtil.Format(State, FacilitiesJsonContext.Default.FacilityState))
            .Add("equipmentnumbers", EquipmentNumbers)
            .Add("stationnumber", StationId)
            .Add("area", area);
    }
}
