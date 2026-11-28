using System.Text.Json.Serialization;
using DotBahn.Ai.Input;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Facilities.Models;
using DotBahn.Facilities.Models.Enumerations;
using DotBahn.Stations.Models;

namespace DotBahn.Ai;

/// <summary>
/// Source-generated JSON metadata for AI tool contracts.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(StationSearchInput))]
[JsonSerializable(typeof(BoardToolInput))]
[JsonSerializable(typeof(FacilityStatusInput))]
[JsonSerializable(typeof(StationIdentifierKind))]
[JsonSerializable(typeof(FacilityType))]
[JsonSerializable(typeof(FacilityState))]
[JsonSerializable(typeof(ToolError))]
[JsonSerializable(typeof(ToolResult<IReadOnlyList<Station>>))]
[JsonSerializable(typeof(ToolResult<IReadOnlyList<Facility>>))]
[JsonSerializable(typeof(ToolResult<Board<DepartureEntry>>))]
[JsonSerializable(typeof(ToolResult<Board<ArrivalEntry>>))]
internal sealed partial class AiJsonContext : JsonSerializerContext;
