using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using UObject.ObjectModel;
using UObject.Properties;

namespace UObject.JSON
{
    [PublicAPI]
    public class UnrealObjectConverter : JsonConverter<UnrealObject>
    {
        public override UnrealObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            new UnrealObject
            {
                Value = JsonSerializer.Deserialize<Dictionary<string, AbstractProperty>>(ref reader, options)!
            };

        public override void Write(Utf8JsonWriter writer, UnrealObject value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value.Value, options);
    }
}
