using System.Text.Json.Serialization;
using DotBahn.Tests.Shared.Contracts;

namespace DotBahn.Tests.Shared;

/// <summary>
/// Shared source-generated JSON context for all tests.
/// </summary>
[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(SimpleDto))]
[JsonSerializable(typeof(AlwaysNullDto))]
[JsonSerializable(typeof(PostRequestDto))]
[JsonSerializable(typeof(TestEnum))]
public partial class TestsJsonContext : JsonSerializerContext;
