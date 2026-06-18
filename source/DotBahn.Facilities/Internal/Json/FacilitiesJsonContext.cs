using System.Text.Json.Serialization;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Facilities.Internal.Json;

/// <summary>
/// Source-generated JSON serialization metadata for Facilities enum types.
/// </summary>
[JsonSerializable(typeof(FacilityState))]
[JsonSerializable(typeof(FacilityType))]
internal sealed partial class FacilitiesJsonContext : JsonSerializerContext;
