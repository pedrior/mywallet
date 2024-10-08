using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyWallet.Shared.Serialization;

public sealed class TrimStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.Trim();

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
}