using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.Generics;
using UObject.JSON;

namespace UObject.Properties
{
    [PublicAPI]
    public class SetProperty : AbstractProperty, IArrayValueType<object?>
    {
        public Name SetType { get; set; } = new Name();

        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public object? Value { get; set; }

        public int Unknown { get; set; }

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Deserialize(buffer, asset, ref cursor, mode);
            SetType.Deserialize(buffer, asset, ref cursor);
            Debug.WriteLine($"SetProperty type is {SetType}");
            Guid.Deserialize(buffer, asset, ref cursor);
#if DEBUG
            var start = cursor;
#endif
            Unknown  = SpanHelper.ReadLittleInt(buffer, ref cursor);
            Logger.Assert(Unknown == 0, "Unknown == 0");
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var value = new HashSet<object?>();

            if (count > 0)
            {
                if (SetType == "StructProperty")
                {
                    // TODO: Struct types are not serialized!
                    for (var i = 0; i < count; ++i) value.Add(ObjectSerializer.DeserializeStruct(buffer, asset, "None", ref cursor));
                }
                else
                {
                    var arrayMode                                                                                            = SerializationMode.Array;
                    if (SetType == "ByteProperty" && Tag?.Size > 0 && (Tag?.Size - 8) / count == 1) arrayMode |= SerializationMode.PureByteArray;
                    for (var i = 0; i < count; ++i) value.Add(ObjectSerializer.DeserializeProperty(buffer, asset, Tag ?? new PropertyTag(), SetType, cursor, ref cursor, arrayMode));
                }
            }
            
            Value = value;
#if DEBUG
            if (SetType != "StructProperty" && cursor != start + Tag?.Size)
                throw new InvalidOperationException("ARRAY SIZE OFFSHOOT");
#endif
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Serialize(ref buffer, asset, ref cursor, mode);
            SetType.Serialize(ref buffer, asset, ref cursor);
            Guid.Serialize(ref buffer, asset, ref cursor);
            switch (Value)
            {
                case List<object?> list:
                {
                    SpanHelper.WriteLittleInt(ref buffer, list.Count, ref cursor);
                    foreach (AbstractProperty? prop in list)
                        prop?.Serialize(ref buffer, asset, ref cursor);
                    break;
                }
                case StructProperty structProperty:
                    structProperty.Serialize(ref buffer, asset, ref cursor, SerializationMode.Array);
                    break;
            }

            // TODO case for generic UObject intead of struct or array?
        }
        
        public override string ToString() => $"{nameof(SetProperty)}[{SetType}]";
    }
}
