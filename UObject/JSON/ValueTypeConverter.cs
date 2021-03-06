using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace UObject.JSON
{
    [PublicAPI]
    public class ValueTypeConverter<T> : JsonConverter<IValueType<T>>
    {
        public override IValueType<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!(Activator.CreateInstance(typeToConvert) is IValueType<T> valueType)) throw new JsonException();
            valueType.Value = (T) JsonSerializer.Deserialize(ref reader, typeof(T), options)!;
            return valueType;
        }

        public override void Write(Utf8JsonWriter writer, IValueType<T> value, JsonSerializerOptions options)
        {
            if(value.Value == null) writer.WriteNullValue();
            else JsonSerializer.Serialize(writer, value.Value, value.Value.GetType(), options);
        }
    }
}
