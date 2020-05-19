using System;
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
        public int LocalizeFlag { get; set; }
        public byte KeyPresent { get; set; }
        public int TextPresent { get; set; }
        public PropertyGuid ValueGuid { get; set; } = new PropertyGuid();
        public string? Namespace { get; set; }
        public string? Hashkey { get; set; }
        public string? Value { get; set; }

        public override string? ToString() => Value;

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            LocalizeFlag = SpanHelper.ReadLittleInt(buffer, ref cursor);
            KeyPresent = SpanHelper.ReadByte(buffer, ref cursor);
            TextPresent = SpanHelper.ReadLittleInt(buffer, ref cursor);
            // TODO: 4.18 serialized uasset with TextProperty in array/struct
            if (Tag?.Size > 5 || mode != SerializationMode.Normal)
            {
                if (KeyPresent == 0)
                {
                    if (TextPresent < 0 || TextPresent > 1)
                    {
                        cursor -= 4;
                        Namespace = ObjectSerializer.DeserializeString(buffer, ref cursor);
                    }

                    // TODO: Find a model that has TextPresent > 1.
                    if (TextPresent > 0)
                        ValueGuid.Deserialize(buffer, asset, ref cursor);
                    Hashkey = ObjectSerializer.DeserializeString(buffer, ref cursor);
                }

                if (TextPresent > 0 || KeyPresent == 0)
                {
                    if (TextPresent > 1) Logger.Assert(SpanHelper.ReadLittleInt(buffer, ref cursor) == 0, "SpanHelper.ReadLittleInt(buffer, ref cursor) == 0");
                    Value = ObjectSerializer.DeserializeString(buffer, ref cursor);
                }
            }
            else
            {
                // TODO: Verify, this is needed for one of the cases. KeyPresent is 0xFF.
                if (KeyPresent == 0xFF) cursor -= 4;
            }
        }

        // TODO - redo for serialization. Do we update flags after reading back from JSON
        // or do we just write flags based on the state of the values (key, namespace, textvalue)?
        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor, mode);
            SpanHelper.WriteLittleInt(ref buffer, LocalizeFlag, ref cursor);
            SpanHelper.WriteByte(ref buffer, KeyPresent, ref cursor);
            if (Tag?.Size > 5 || mode != SerializationMode.Normal)
            {
                SpanHelper.WriteLittleInt(ref buffer, TextPresent, ref cursor);
                if ((LocalizeFlag & 8) == 8) ValueGuid.Serialize(ref buffer, asset, ref cursor);
                if (KeyPresent != 0xFF)
                {
                    ObjectSerializer.SerializeString(ref buffer, Hashkey ?? string.Empty, ref cursor);
                    ObjectSerializer.SerializeString(ref buffer, Value ?? string.Empty, ref cursor);
                }
            }
            else
            {
                // TODO: Verify, this is needed for one of the cases. KeyPresent is 0xFF.
                if (KeyPresent == 0xFF) SpanHelper.WriteLittleDouble(ref buffer, 0, ref cursor);
            }
        }
    }
}
