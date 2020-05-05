using System;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.JSON;

namespace UObject.Properties
{
    [PublicAPI]
    public class StrProperty : AbstractGuidProperty, IValueType<string?>
    {
        public string? Value { get; set; }

        public override string? ToString() => Value;

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            Value = ObjectSerializer.DeserializeString(buffer, ref cursor);
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor, mode);
            ObjectSerializer.SerializeString(ref buffer, Value ?? String.Empty, ref cursor);
        }
    }
}
