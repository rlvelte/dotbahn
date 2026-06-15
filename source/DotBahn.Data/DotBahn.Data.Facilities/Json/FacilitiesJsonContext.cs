using System.Text.Json.Serialization;

using DotBahn.Data.Facilities.Enumerations;

namespace DotBahn.Data.Facilities.Json;

/// <summary>
/// Source-generated JSON serialization metadata for Data.Facilities enum types.
/// </summary>
[JsonSerializable(typeof(FacilityState))]
[JsonSerializable(typeof(FacilityType))]
public sealed partial class FacilitiesJsonContext : JsonSerializerContext;
