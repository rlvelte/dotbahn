using System.Text.Json.Serialization;
using DotBahn.Tests.Shared.Converters;

namespace DotBahn.Tests.Shared.Contracts;

[JsonConverter(typeof(AlwaysNullConverter))]
public class AlwaysNullDto;
