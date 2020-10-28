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
    public class TextProperty : AbstractGuidProperty, IValueType<object?>
    {
        public struct TextRepresent
        {
            public string? Value { get; set; }
            public Name    Table { get; set; }
            public string? TableKey { get; set; }
        }
        
        public int LocalizeFlag { get; set; }
        public byte KeyPresent { get; set; }
        public int TextPresent { get; set; }
        public Name StringTable { get; set; } = new Name();
        public PropertyGuid ValueGuid { get; set; } = new PropertyGuid();
        public string? Namespace { get; set; }
        public string? Hashkey { get; set; }
        public string? StringValue { get; set; }
        public string? StringTableKey { get; set; }

        public object? Value
        {
            get
            {
                return StringTableKey == null ? (object?) StringValue : new TextRepresent { Value = StringValue, TableKey = StringTableKey, Table = StringTable, };
            }
            set
            {
                switch (value) {
                    case string str: StringValue = str;
                        break;

                    case TextRepresent textRepresent:
                    {
                        StringValue = textRepresent.Value;
                        StringTableKey = textRepresent.TableKey;
                        StringTable = textRepresent.Table;
                        break;
                    }
                }
            }
        } 

        public override string? ToString() => StringValue;

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            LocalizeFlag = SpanHelper.ReadLittleInt(buffer, ref cursor);
            KeyPresent = SpanHelper.ReadByte(buffer, ref cursor);
            // TODO: 4.18 serialized uasset with TextProperty in array/struct
            if (KeyPresent == 0)
            {
                TextPresent = SpanHelper.ReadLittleInt(buffer, ref cursor);
                if (TextPresent < 0 || TextPresent > 1)
                {
                    cursor -= 4;
                    Namespace = ObjectSerializer.DeserializeString(buffer, ref cursor);
                }

                Hashkey = ObjectSerializer.DeserializeString(buffer, ref cursor);
                
                if (LocalizeFlag > 8) // ?
                {
                    ValueGuid.Deserialize(buffer, asset, ref cursor);
                }
            }
            else if(KeyPresent < 0xFF)
            {
                StringTable.Deserialize(buffer, asset, ref cursor);
                StringTableKey = ObjectSerializer.DeserializeString(buffer, ref cursor);
            }

            if (TextPresent > 0 || KeyPresent == 0)
            {
                StringValue = ObjectSerializer.DeserializeString(buffer, ref cursor);
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
                    ObjectSerializer.SerializeString(ref buffer, StringValue ?? string.Empty, ref cursor);
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
