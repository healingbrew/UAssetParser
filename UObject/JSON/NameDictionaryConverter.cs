using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using UObject.Generics;

namespace UObject.JSON
{
    [PublicAPI]
    public class NameDictionaryConverter<T> : JsonConverter<Dictionary<Name, T>>
    {
        public override Dictionary<Name, T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, Dictionary<Name, T> dict, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var (key, value) in dict)
            {
                writer.WritePropertyName(key.Value);
                JsonSerializer.Serialize(writer, value, value?.GetType(), options);
            }

            writer.WriteEndObject();
        }
    }
}
