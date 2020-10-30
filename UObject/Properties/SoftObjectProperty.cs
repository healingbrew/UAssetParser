using System;
using System.Diagnostics;

using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.Generics;

namespace UObject.Properties
{
    [PublicAPI]
    public class SoftObjectProperty : AbstractGuidProperty
    {
        public Name Package { get; set; } = new Name();

        public string? Path { get; set; }

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            Package.Deserialize(buffer, asset, ref cursor);
            Path = ObjectSerializer.DeserializeString(buffer, ref cursor);
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor, mode);
            Package.Serialize(ref buffer, asset, ref cursor);
            ObjectSerializer.SerializeString(ref buffer, Path ?? String.Empty, ref cursor);
        }
    }
}
