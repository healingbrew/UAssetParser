using System;
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
    public class ByteProperty : AbstractProperty, IValueType<object?>
    {
        public Name EnumName { get; set; } = new Name();

        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public object? Value { get; set; }

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            if (mode == SerializationMode.Normal)
            {
                EnumName.Deserialize(buffer, asset, ref cursor);
                Guid.Deserialize(buffer, asset, ref cursor);
            }

            if (mode == SerializationMode.Normal && EnumName.Value == "None" || mode.HasFlag(SerializationMode.PureByteArray))
            {
                Value = SpanHelper.ReadByte(buffer, ref cursor);
            }
            else
            {
                Value = new Name();
                ((Name) Value).Deserialize(buffer, asset, ref cursor);
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor);
            if (mode == SerializationMode.Normal)
            {
                EnumName.Serialize(ref buffer, asset, ref cursor);
                Guid.Serialize(ref buffer, asset, ref cursor);
            }

            if (Value is Name name)
                name.Serialize(ref buffer, asset, ref cursor);
            else
                SpanHelper.WriteByte(ref buffer, (byte) (Value ?? 0), ref cursor);
        }
    }
}
