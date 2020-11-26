using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace UObject.JSON
{
    [PublicAPI]
    public class NoneStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            reader.TokenType switch
            {
                JsonTokenType.Null => "None",
                JsonTokenType.String => reader.GetString()!,
                _ => throw new JsonException()
            };

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == "None" || value == null) writer.WriteNullValue();
            else writer.WriteStringValue(value.AsSpan());
        }
    }
}
