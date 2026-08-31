using System.Text.Json.Serialization;
using DotBahn.Stations.Models.Enumerations;

namespace DotBahn.Stations.Internal.Json;

/// <summary>
/// Source-generated JSON serialization metadata for Data.Stations enum types
/// </summary>
[JsonSerializable(typeof(FederalState))]
[JsonSerializable(typeof(LogicalOperator))]
public sealed partial class StationsJsonContext : JsonSerializerContext;
