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
    public class MapProperty : AbstractProperty, IArrayValueType<Dictionary<string, object?>>
    {
        public Name KeyType { get; set; } = new Name();
        public Name ValueType { get; set; } = new Name();

        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public int Reserved { get; set; }
        public Dictionary<string, object?> Value { get; set; } = new Dictionary<string, object?>();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Deserialize(buffer, asset, ref cursor, mode);
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(MapProperty)} at {cursor:X}");
            KeyType.Deserialize(buffer, asset, ref cursor);
            ValueType.Deserialize(buffer, asset, ref cursor);
            Guid.Deserialize(buffer, asset, ref cursor);
            Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
            Logger.Assert(Reserved == 0, "Reserved == 0");
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var valueMode = SerializationMode.Map;
            if (ValueType == "ByteProperty" && Tag?.Size != 1) valueMode &= SerializationMode.ByteAsEnum;
            for (var i = 0; i < count; ++i)
            {
                var key = ObjectSerializer.DeserializeProperty(buffer, asset, Tag ?? new PropertyTag(), KeyType, cursor, ref cursor, SerializationMode.Map);

                if (ValueType == "StructProperty")
                    Value[key.ToString() ?? $"@$None_{cursor:X}"] = ObjectSerializer.DeserializeStruct(buffer, asset, "None", ref cursor);
                else
                    Value[key.ToString() ?? $"@$None_{cursor:X}"] = ObjectSerializer.DeserializeProperty(buffer, asset, Tag ?? new PropertyTag(), ValueType, cursor, ref cursor, valueMode);
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Serialize(ref buffer, asset, ref cursor, mode);
            KeyType.Serialize(ref buffer, asset, ref cursor);
            ValueType.Serialize(ref buffer, asset, ref cursor);
            Guid.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Reserved, ref cursor);
        }
    }
}
