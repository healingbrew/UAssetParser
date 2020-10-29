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
    public class ObjectProperty : AbstractProperty, IValueType<PackageIndex>
    {
        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public PackageIndex Value { get; set; } = new PackageIndex();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(ObjectProperty)} at {cursor:X}");
            if (mode == SerializationMode.Normal) Guid.Deserialize(buffer, asset, ref cursor);
            Value.Deserialize(buffer, asset, ref cursor);
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor, mode);
            if (mode == SerializationMode.Normal) Guid.Serialize(ref buffer, asset, ref cursor);
            Value.Serialize(ref buffer, asset, ref cursor);
        }
    }
}
