using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Facilities.Internal.Contracts;
using DotBahn.Facilities.Models.Enumerations;

namespace DotBahn.Facilities.Internal.Json;

/// <summary>
/// Source-generated JSON serialization metadata for facility contracts and enums
/// </summary>
[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FacilityContract))]
[JsonSerializable(typeof(List<FacilityContract>))]
[JsonSerializable(typeof(FacilityState))]
[JsonSerializable(typeof(FacilityType))]
internal sealed partial class FacilitiesJsonContext : JsonSerializerContext;
