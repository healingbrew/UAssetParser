using System;
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
    public class TextProperty : AbstractProperty, IValueType<string>
    {
        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public long StreamOffset { get; set; }

        [JsonIgnore]
        public PropertyGuid Key { get; set; } = new PropertyGuid();

        public string Hash { get; set; } = "None";

        public string Value { get; set; } = "None";

        public override string ToString() => Value;

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            if (mode != SerializationMode.Normal)
                throw new NotImplementedException("Array order for TextProperty is different");
            base.Deserialize(buffer, asset, ref cursor, mode);
            var padSkip = 1;
            if (Tag?.Size > 7)
            {
                Guid.Deserialize(buffer, asset, ref cursor);
                StreamOffset = SpanHelper.ReadLittleLong(buffer, ref cursor);
                Key.Deserialize(buffer, asset, ref cursor);
                Hash = ObjectSerializer.DeserializeString(buffer, ref cursor);
                padSkip = -1;
            }
            Value = ObjectSerializer.DeserializeString(buffer, ref cursor);
            cursor += padSkip;
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            if (mode != SerializationMode.Normal)
                throw new NotImplementedException("Array order for TextProperty is different");

            base.Serialize(ref buffer, asset, ref cursor);
            if (Tag?.Size > 7)
            {
                Guid.Serialize(ref buffer, asset, ref cursor);
                SpanHelper.WriteLittleLong(ref buffer, StreamOffset, ref cursor);
                Key.Serialize(ref buffer, asset, ref cursor);
                ObjectSerializer.SerializeString(ref buffer, Hash, ref cursor);
            }
            ObjectSerializer.SerializeString(ref buffer, Value, ref cursor);
            cursor -= 1;
            if (Tag?.Size < 8)
                SpanHelper.WriteLittleShort(ref buffer, -256, ref cursor);
        }
    }
}
