using System;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;

namespace UObject.Generics
{
    [PublicAPI]
    public class PropertyTag : ISerializableObject
    {
        public Name Name { get; set; } = new Name();
        public Name Type { get; set; } = new Name();
        public int Size { get; set; }
        public int Index { get; set; }

        public virtual void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Name.Deserialize(buffer, asset, ref cursor);
            Type.Deserialize(buffer, asset, ref cursor);
            Size = SpanHelper.ReadLittleInt(buffer, ref cursor);
            Index = SpanHelper.ReadLittleInt(buffer, ref cursor);
        }

        public virtual void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => throw new NotImplementedException();

        public override string ToString() => $"<{Type}> {Name}";
    }
}
