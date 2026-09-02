using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Stations.Internal.Contracts;
using DotBahn.Stations.Models.Enumerations;

namespace DotBahn.Stations.Internal.Json;

/// <summary>
/// Source-generated JSON serialization metadata for station contracts and enums
/// </summary>
[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(StationsResponseContract))]
[JsonSerializable(typeof(StationContract))]
[JsonSerializable(typeof(MailingAddressContract))]
[JsonSerializable(typeof(RegionalAreaContract))]
[JsonSerializable(typeof(Ril100IdentifierContract))]
[JsonSerializable(typeof(EvaNumberContract))]
[JsonSerializable(typeof(GeographicCoordinatesContract))]
[JsonSerializable(typeof(FederalState))]
[JsonSerializable(typeof(LogicalOperator))]
internal sealed partial class StationsJsonContext : JsonSerializerContext;
