using System;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Generics;

namespace UObject.Asset
{
    [PublicAPI]
    public class NameEntry : ISerializableObject
    {
        // Why is it nullable? NameEntries never null because they are hash table essentially
        public string? Name { get; set; }
        public ushort NonCasePreservingHash { get; set; }
        public ushort CasePreservingHash { get; set; }

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Name = ObjectSerializer.DeserializeString(buffer, ref cursor);
            NonCasePreservingHash = SpanHelper.ReadLittleUShort(buffer, ref cursor);
            CasePreservingHash = SpanHelper.ReadLittleUShort(buffer, ref cursor);
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ObjectSerializer.SerializeString(ref buffer, Name ?? String.Empty, ref cursor);
            SpanHelper.WriteLittleUShort(ref buffer, NonCasePreservingHash, ref cursor);
            SpanHelper.WriteLittleUShort(ref buffer, CasePreservingHash, ref cursor);
        }

        public override string ToString() => Name ?? base.ToString() ?? "None";

        public override int GetHashCode() => CasePreservingHash;
    }
}
