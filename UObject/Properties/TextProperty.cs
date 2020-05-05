using System;
using System.Collections.Generic;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.Generics;
using UObject.JSON;

namespace UObject.Properties
{
    [PublicAPI]
    public class TextProperty : AbstractGuidProperty, IValueType<string?>
    {
#if DEBUG
        public static HashSet<int> Unknown3Set = new HashSet<int>();
#endif
        public int SerializationFlags { get; set; }
        public byte Flags { get; set; }
        public int Unknown3 { get; set; }
        public PropertyGuid ValueGuid { get; set; } = new PropertyGuid();
        public string? Hash { get; set; }
        public string? Value { get; set; }

        public override string? ToString() => Value;

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            SerializationFlags = SpanHelper.ReadLittleInt(buffer, ref cursor);
            Logger.Assert(SerializationFlags == 0 || SerializationFlags == 8, "SerializationFlags == 0 || SerializationFlags == 8", SerializationFlags.ToString());
            // Unknown3 and Flags are combined as an int32. There's an excess zero guid.
            Flags = SpanHelper.ReadByte(buffer, ref cursor);
            if (mode != SerializationMode.Normal || Tag?.Size > 5)
            {
                Unknown3 = SpanHelper.ReadLittleInt(buffer, ref cursor);
#if DEBUG
                Unknown3Set.Add(Unknown3);
#endif
                if ((SerializationFlags & 8) == 8) ValueGuid.Deserialize(buffer, asset, ref cursor);
                if (Flags != 0xFF)
                {
                    Hash = ObjectSerializer.DeserializeString(buffer, ref cursor);
                    Value = ObjectSerializer.DeserializeString(buffer, ref cursor);
                }
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor, mode);
            SpanHelper.WriteLittleInt(ref buffer, SerializationFlags, ref cursor);
            SpanHelper.WriteByte(ref buffer, Flags, ref cursor);
            if (mode != SerializationMode.Normal || Tag?.Size > 5)
            {
                SpanHelper.WriteLittleInt(ref buffer, Unknown3, ref cursor);
                if ((SerializationFlags & 8) == 8) ValueGuid.Serialize(ref buffer, asset, ref cursor);
                if (Flags != 0xFF)
                {
                    ObjectSerializer.SerializeString(ref buffer, Hash ?? string.Empty, ref cursor);
                    ObjectSerializer.SerializeString(ref buffer, Value ?? string.Empty, ref cursor);
                }
            }
        }
    }
}
