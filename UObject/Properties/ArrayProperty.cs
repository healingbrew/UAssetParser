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
    public class ArrayProperty : AbstractProperty, IArrayValueType<object?>
    {
        public Name ArrayType { get; set; } = new Name();

        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public object? Value { get; set; }

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Deserialize(buffer, asset, ref cursor, mode);
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(ArrayProperty)} at {cursor:X}");
            ArrayType.Deserialize(buffer, asset, ref cursor);
            Guid.Deserialize(buffer, asset, ref cursor);
#if DEBUG
            var start = cursor;
#endif
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var value = new List<object?>();
            if (ArrayType == "StructProperty")
            {
                var structTag = new PropertyTag();
                structTag.Deserialize(buffer, asset, ref cursor);
                var structProperty = ObjectSerializer.DeserializeProperty(buffer, asset, structTag, ArrayType, cursor, ref cursor, SerializationMode.Array) as StructProperty ?? throw new InvalidDataException();
                for (var i = 0; i < count; ++i) value.Add(ObjectSerializer.DeserializeStruct(buffer, asset, structProperty?.StructName! ?? "None", ref cursor));
                structProperty!.Value = value;
                Value = structProperty;
            }
            else
            {
                var arrayMode = SerializationMode.Array;
                if (ArrayType == "ByteProperty" && count > 0 && Tag?.Size > 0 && (Tag?.Size - 4) / count == 1) arrayMode |= SerializationMode.PureByteArray;
                for (var i = 0; i < count; ++i) value.Add(ObjectSerializer.DeserializeProperty(buffer, asset, Tag ?? new PropertyTag(), ArrayType, cursor, ref cursor, arrayMode));
            }
            Value = value;
#if DEBUG
            if (ArrayType != "StructProperty" && cursor != start + Tag?.Size)
                throw new InvalidOperationException("ARRAY SIZE OFFSHOOT");
#endif
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            Logger.Assert(mode == SerializationMode.Normal, "mode == SerializationMode.Normal");
            base.Serialize(ref buffer, asset, ref cursor, mode);
            ArrayType.Serialize(ref buffer, asset, ref cursor);
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
    }
}
