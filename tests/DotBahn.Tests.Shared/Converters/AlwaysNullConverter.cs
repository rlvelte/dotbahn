using System.Text.Json;
using System.Text.Json.Serialization;
using DotBahn.Tests.Shared.Contracts;

namespace DotBahn.Tests.Shared.Converters;

public class AlwaysNullConverter : JsonConverter<AlwaysNullDto> {
    /// <inheritdoc />
    public override AlwaysNullDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => null;

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, AlwaysNullDto value, JsonSerializerOptions options) => writer.WriteNullValue();
}
