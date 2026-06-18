using System.Text.Json.Serialization;

using DotBahn.Facilities.Enumerations;

namespace DotBahn.Facilities.Json;

/// <summary>
/// Source-generated JSON serialization metadata for Data.Facilities enum types.
/// </summary>
[JsonSerializable(typeof(FacilityState))]
[JsonSerializable(typeof(FacilityType))]
public sealed partial class FacilitiesJsonContext : JsonSerializerContext;
