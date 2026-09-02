using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DotBahn.Timetables.Internal.Contracts;
using DotBahn.Timetables.Models.Enumerations;

namespace DotBahn.Timetables.Internal.Json;

/// <summary>
/// Source-generated JSON serialization metadata for timetable contracts and enums
/// </summary>
[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(TimetableResponseContract))]
[JsonSerializable(typeof(StopDataContract))]
[JsonSerializable(typeof(EventContract))]
[JsonSerializable(typeof(MessageContract))]
[JsonSerializable(typeof(TripInfoContract))]
[JsonSerializable(typeof(EventStatus))]
[JsonSerializable(typeof(MessageType))]
[JsonSerializable(typeof(TripType))]
internal sealed partial class TimetableJsonContext : JsonSerializerContext;
