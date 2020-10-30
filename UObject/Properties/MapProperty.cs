using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.Generics;
using UObject.JSON;

namespace UObject.Properties
{
    [PublicAPI]
    public class MapProperty : AbstractProperty, IArrayValueType<Dictionary<object, object?>>
    {
        public Name KeyType { get; set; } = new Name();
        public Name ValueType { get; set; } = new Name();

        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();
        public Dictionary<object, object?> Value { get; set; } = new Dictionary<object, object?>();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Deserialize(buffer, asset, ref cursor, mode);
            KeyType.Deserialize(buffer, asset, ref cursor);
            ValueType.Deserialize(buffer, asset, ref cursor);
            Debug.WriteLine($"MapProperty key type is {KeyType} and value type is {ValueType}");
            Guid.Deserialize(buffer, asset, ref cursor);
            var nullCount = SpanHelper.ReadLittleInt(buffer, ref cursor);
            for (var i = 0; i < nullCount; ++i)
            {
                var key = ObjectSerializer.DeserializeProperty(buffer, asset, Tag ?? new PropertyTag(), KeyType, cursor, ref cursor, SerializationMode.Map);
                Value[key] = null;
            }

            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var valueMode = SerializationMode.Map;
            if (ValueType == "ByteProperty" && Tag?.Size != 1) valueMode &= SerializationMode.ByteAsEnum;
            for (var i = 0; i < count; ++i)
            {
                var key = ObjectSerializer.DeserializeProperty(buffer, asset, Tag ?? new PropertyTag(), KeyType, cursor, ref cursor, SerializationMode.Map);

                if (ValueType == "StructProperty")
                    Value[key] = ObjectSerializer.DeserializeStruct(buffer, asset, "None", ref cursor);
                else
                    Value[key] = ObjectSerializer.DeserializeProperty(buffer, asset, Tag ?? new PropertyTag(), ValueType, cursor, ref cursor, valueMode);
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Serialize(ref buffer, asset, ref cursor, mode);
            KeyType.Serialize(ref buffer, asset, ref cursor);
            ValueType.Serialize(ref buffer, asset, ref cursor);
            Guid.Serialize(ref buffer, asset, ref cursor);
        }
        
        public override string ToString() => $"{nameof(MapProperty)}[{KeyType}, {ValueType}]";
    }
}
