using System;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Generics;

namespace UObject.Asset
{
    [PublicAPI]
    public class CustomVersion : ISerializableObject
    {
        public Guid Key { get; set; }
        public int Version { get; set; }

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(CustomVersion)} at {cursor:X}");
            if (asset.Summary.LegacyFileVersion < -2 && asset.Summary.LegacyFileVersion >= -5) Key = SpanHelper.ReadStruct<Guid>(buffer, ref cursor);

            Version = SpanHelper.ReadLittleInt(buffer, ref cursor);
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            if (asset.Summary.LegacyFileVersion < -2 && asset.Summary.LegacyFileVersion >= -5) SpanHelper.WriteStruct(ref buffer, Key, ref cursor);

            SpanHelper.WriteLittleInt(ref buffer, Version, ref cursor);
        }
    }
}
