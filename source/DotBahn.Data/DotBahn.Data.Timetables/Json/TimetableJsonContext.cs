using System.Text.Json.Serialization;

using DotBahn.Data.Timetables.Enumerations;

namespace DotBahn.Data.Timetables.Json;

/// <summary>
/// Source-generated JSON serialization metadata for Data.Timetables enum types.
/// </summary>
[JsonSerializable(typeof(EventStatus))]
[JsonSerializable(typeof(MessageType))]
[JsonSerializable(typeof(TripType))]
public sealed partial class TimetableJsonContext : JsonSerializerContext;
