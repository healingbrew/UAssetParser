using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.Generics;
using UObject.JSON;
using UObject.ObjectModel;

namespace UObject.Properties
{
    [PublicAPI]
    public class StructProperty : AbstractProperty, IArrayValueType<object?>
    {
        public Name StructType { get; set; } = new Name();
        public Guid StructGuid { get; set; } = System.Guid.Empty;

        [JsonIgnore]
        public PropertyGuid Guid { get; set; } = new PropertyGuid();

        public object? Value { get; set; }

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            StructType.Deserialize(buffer, asset, ref cursor);
            Debug.WriteLineIf(Debugger.IsAttached, $"Struct name is {StructType}");
            if (mode == SerializationMode.Normal || mode == SerializationMode.Array)
            {
                StructGuid = SpanHelper.ReadStruct<Guid>(buffer, ref cursor);
                Guid.Deserialize(buffer, asset, ref cursor);
            }

            if (mode == SerializationMode.Normal) Value = ObjectSerializer.DeserializeStruct(buffer, asset, StructType, ref cursor);
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor, mode);
            StructType.Serialize(ref buffer, asset, ref cursor);
            if (mode == SerializationMode.Normal || mode == SerializationMode.Array)
            {
                SpanHelper.WriteStruct(ref buffer, StructGuid, ref cursor);
                Guid.Serialize(ref buffer, asset, ref cursor);
            }

            if (mode == SerializationMode.Normal)
            {
                if (Value is UnrealObject uobject)
                    uobject.Serialize(ref buffer, asset, ref cursor);
                else if (Value != null)
                    unsafe
                    {
                        SpanHelper.EnsureSpace(ref buffer, cursor + Marshal.SizeOf(Value.GetType()));
                        fixed (byte* pin = &buffer.Span.Slice(cursor).GetPinnableReference())
                        {
                            var addr = (IntPtr) pin;
                            Marshal.StructureToPtr(Value, addr, true);
                        }
                    }
            }
        }

        public override string ToString() => $"{nameof(StructProperty)}{{{StructType}}}";
    }
}
