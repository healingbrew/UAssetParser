using DragonLib.IO;
using System;
using UObject.Asset;
using UObject.Generics;
using UObject.JSON;

namespace UObject.ObjectModel
{
    public class UnversionedFragment : ISerializableObject, IValueType<ushort>
    {
        public ushort Value { get; set; }
        public int Skip => Value & 0x7F;
        public bool HasZero => (Value & 0x80) != 0;
        public bool IsLast => (Value & 0x100) != 0;
        public int Count => Value >> 9;
        
        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Value = SpanHelper.ReadLittleUShort(buffer, ref cursor);
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            SpanHelper.WriteLittleUShort(ref buffer, Value, ref cursor);
        }
    }
}
