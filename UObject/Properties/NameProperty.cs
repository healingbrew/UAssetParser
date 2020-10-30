using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.Generics;
using UObject.JSON;

namespace UObject.Properties
{
    [PublicAPI]
    public class NameProperty : AbstractProperty, IValueType<Name>
    {
        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public Name Value { get; set; } = new Name();

        public override string ToString() => Value;

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            if (mode == SerializationMode.Normal) Guid.Deserialize(buffer, asset, ref cursor);
            Value.Deserialize(buffer, asset, ref cursor);
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor);
            if (mode == SerializationMode.Normal) Guid.Serialize(ref buffer, asset, ref cursor);
            Value.Serialize(ref buffer, asset, ref cursor);
        }
    }
}
