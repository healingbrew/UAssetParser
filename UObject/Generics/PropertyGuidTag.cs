using System;
using System.Diagnostics;

using JetBrains.Annotations;
using UObject.Asset;

namespace UObject.Generics
{
    [PublicAPI]
    public class PropertyGuidTag : PropertyTag
    {
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Deserialize(buffer, asset, ref cursor);
            Guid.Deserialize(buffer, asset, ref cursor);
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Serialize(ref buffer, asset, ref cursor);
            Guid.Serialize(ref buffer, asset, ref cursor);
        }
    }
}
