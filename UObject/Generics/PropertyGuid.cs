using System;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;

namespace UObject.Generics
{
    [PublicAPI]
    public class PropertyGuid : ISerializableObject
    {
        public bool HasGuid => Guid != Guid.Empty;

        public Guid Guid { get; set; } = Guid.Empty;

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(PropertyGuid)} at {cursor:X}");
            var boolByte = SpanHelper.ReadByte(buffer, ref cursor);
            Logger.Assert(boolByte       <= 1, "boolByte <= 1");
            var hasGuid       = boolByte == 1;
            if (hasGuid) Guid = SpanHelper.ReadStruct<Guid>(buffer, ref cursor);
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            SpanHelper.WriteByte(ref buffer, (byte) (HasGuid ? 1 : 0), ref cursor);
            if (HasGuid) SpanHelper.WriteStruct(ref buffer, Guid, ref cursor);
        }
    }
}
