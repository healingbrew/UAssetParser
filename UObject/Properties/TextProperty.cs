using System;
using System.Diagnostics;

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
        public int LocalizeFlag { get; set; }
        public byte KeyPresent { get; set; }
        public int TextPresent { get; set; }
        public PropertyGuid ValueGuid { get; set; } = new PropertyGuid();
        public string? Namespace { get; set; }
        public string? Hashkey { get; set; }
        public string? StringValue { get; set; }
        public Reference StringTable { get; set; } = new Reference();
        public int Unknown { get; set; }
        public object? Value
        {
            get
            {
                return StringTable.Key == null ? (object?) StringValue : StringTable;
            }
            set
            {
                switch (value) {
                    case string str: StringValue = str;
                        break;

                    case Reference reference:
                    {
                        StringTable = reference;
                        break;
                    }
                }
            }
        } 

        public override string? ToString() => StringValue;

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(TextProperty)} at {cursor:X}");
            var estimatedEnd = cursor + (Tag?.Size ?? 0);
            LocalizeFlag = SpanHelper.ReadLittleInt(buffer, ref cursor);
            KeyPresent = SpanHelper.ReadByte(buffer, ref cursor);
            // TODO: 4.18 serialized uasset with TextProperty in array/struct
            if (KeyPresent == 0)
            {
                Namespace = ObjectSerializer.DeserializeString(buffer, ref cursor);
                Hashkey = ObjectSerializer.DeserializeString(buffer, ref cursor);
                
                if (LocalizeFlag != 0 && (LocalizeFlag ^ 8) > 0 && (LocalizeFlag ^ 2) > 0) // ?
                {
                    ValueGuid.Deserialize(buffer, asset, ref cursor);
                }
                
                StringValue = ObjectSerializer.DeserializeString(buffer, ref cursor);
            }
            else if(KeyPresent < 0xFF)
            {
                StringTable.Deserialize(buffer, asset, ref cursor);
            }
            
            if (KeyPresent == 0xFF && (estimatedEnd > cursor || (mode != SerializationMode.Normal && asset.Summary.FileVersionUE4 >= 515)))
            {
                Unknown = SpanHelper.ReadLittleInt(buffer, ref cursor);
                
                if ((LocalizeFlag & 2) == 2)
                {
                    StringValue = ObjectSerializer.DeserializeString(buffer, ref cursor);
                }
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
