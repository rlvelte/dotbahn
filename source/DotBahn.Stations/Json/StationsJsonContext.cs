using System.Text.Json.Serialization;

using DotBahn.Stations.Enumerations;

namespace DotBahn.Stations.Json;

/// <summary>
/// Source-generated JSON serialization metadata for Data.Stations enum types.
/// </summary>
[JsonSerializable(typeof(FederalState))]
[JsonSerializable(typeof(LogicalOperator))]
public sealed partial class StationsJsonContext : JsonSerializerContext;
