using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Enum;
using UObject.GameModel.FF7.ObjectModel;
using UObject.Generics;
using UObject.ObjectModel;
using UObject.Properties;
using UObject.Structs;

namespace UObject
{
    [PublicAPI]
    public static class ObjectSerializer
    {
        public static Dictionary<string, Type> PropertyTypes { get; } = new Dictionary<string, Type>
        {
            { nameof(ObjectProperty), typeof(ObjectProperty) },
            { nameof(SoftObjectProperty), typeof(SoftObjectProperty) },
            { nameof(StructProperty), typeof(StructProperty) },
            { nameof(NameProperty), typeof(NameProperty) },
            { nameof(StrProperty), typeof(StrProperty) },
            { nameof(TextProperty), typeof(TextProperty) },
            { nameof(ArrayProperty), typeof(ArrayProperty) },
            { nameof(MapProperty), typeof(MapProperty) },
            { nameof(EnumProperty), typeof(EnumProperty) },
            { nameof(ByteProperty), typeof(ByteProperty) },
            { "ShortProperty", typeof(Int16Property) },
            { "UShortProperty", typeof(UInt16Property) },
            { "IntProperty", typeof(Int32Property) },
            { "UIntProperty", typeof(UInt32Property) },
            { "LongProperty", typeof(Int64Property) },
            { "ULongProperty", typeof(UInt64Property) },
            { nameof(Int16Property), typeof(Int16Property) },
            { nameof(UInt16Property), typeof(UInt16Property) },
            { nameof(Int32Property), typeof(Int32Property) },
            { nameof(UInt32Property), typeof(UInt32Property) },
            { nameof(Int64Property), typeof(Int64Property) },
            { nameof(UInt64Property), typeof(UInt64Property) },
            { nameof(FloatProperty), typeof(FloatProperty) },
            { nameof(BoolProperty), typeof(BoolProperty) }
        };

        public static Dictionary<string, Type> ClassTypes { get; } = new Dictionary<string, Type>
        {
            { nameof(DataTable), typeof(DataTable) },
            { nameof(StringTable), typeof(StringTable) },
            { nameof(EndTextResource), typeof(EndTextResource) }
        };

        public static AssetFile Deserialize(Span<byte> uasset, Span<byte> uexp, AssetFileOptions options) => new AssetFile(uasset, uexp, options);

        public static Span<byte> SerializeExports(ref PackageFileSummary summary, List<UnrealObject> uexp) => throw new NotImplementedException();

        public static Span<byte> SerializeSummary(PackageFileSummary summary, List<UnrealObject> uasset) => throw new NotImplementedException();

        public static string? DeserializeString(Span<byte> buffer, ref int cursor)
        {
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var str = default(string);
            if (count > 0)
            {
                str = count == 1 ? string.Empty : Encoding.UTF8.GetString(buffer.Slice(cursor, count - 1));
                cursor += count;
            }
            else if (count < 0)
            {
                str = count == -1 ? string.Empty : Encoding.Unicode.GetString(buffer.Slice(cursor, (0 - count) * 2 - 2));
                cursor += (0 - count) * 2;
            }

            return str;
        }

        public static AbstractProperty DeserializeProperty(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            var start = cursor;
            var tag = new PropertyTag();
            tag.Deserialize(buffer, asset, ref cursor);
            var tmp = cursor;
            cursor = start;
            return DeserializeProperty(buffer, asset, tag, tag.Type, tmp, ref cursor, SerializationMode.Normal);
        }

        public static AbstractProperty DeserializeProperty(Span<byte> buffer, AssetFile asset, PropertyTag tag, Name serializationType, int offset, ref int cursor, SerializationMode mode)
        {
            if (serializationType == null) throw new InvalidDataException();
            if (!PropertyTypes.TryGetValue(serializationType, out var propertyType))
            {
                Logger.Error("UObject", $"No Handler for property {tag.Name.Value} which has the type {serializationType.Value} at offset {offset:X} (size {tag.Size})");
                throw new NotImplementedException($"No Handler for property {tag.Name.Value} which has the type {serializationType.Value} at offset {offset:X} (size {tag.Size})");
            }

            if (!(Activator.CreateInstance(propertyType) is AbstractProperty instance)) throw new InvalidDataException();
            instance.Deserialize(buffer, asset, ref cursor, mode);
            return instance;
        }

        public static T[] DeserializeProperties<T>(Span<byte> buffer, AssetFile asset, int count, ref int cursor) where T : ISerializableObject, new()
        {
            var instances = AllocateProperties<T>(count);
            DeserializeProperties(buffer, asset, instances, ref cursor);
            return instances;
        }

        public static T[] AllocateProperties<T>(int count) where T : ISerializableObject, new()
        {
            var instances = new T[count];
            for (var i = 0; i < count; ++i) instances[i] = new T();
            return instances;
        }

        public static void DeserializeProperties<T>(Span<byte> buffer, AssetFile asset, T[] instances, ref int cursor) where T : ISerializableObject, new()
        {
            foreach (var instance in instances) instance.Deserialize(buffer, asset, ref cursor);
        }

        public static void SerializeProperties<T>(ref Memory<byte> buffer, AssetFile asset, T[] instances, ref int cursor) where T : ISerializableObject, new()
        {
            foreach (var instance in instances) instance.Serialize(ref buffer, asset, ref cursor);
        }

        public static void SerializeString(ref Memory<byte> buffer, string text, ref int cursor)
        {
            if (text == null) return;
            if (text == string.Empty)
            {
                SpanHelper.WriteLittleInt(ref buffer, 1, ref cursor);
                cursor += 1;
            }

            var length = text.Length + 1;
            var utf16 = false;
            var bufferLength = Encoding.UTF8.GetByteCount(text) + 1;
            if (bufferLength != length)
            {
                utf16 = true;
                bufferLength = Encoding.Unicode.GetByteCount(text) + 2;
            }

            var span = new Span<char>(text.ToCharArray());
            SpanHelper.WriteLittleInt(ref buffer, utf16 ? 0 - length : length, ref cursor);
            SpanHelper.EnsureSpace(ref buffer, cursor + bufferLength);
            if (utf16) Encoding.Unicode.GetBytes(span, buffer.Slice(cursor).Span);
            else Encoding.UTF8.GetBytes(span, buffer.Slice(cursor).Span);
            cursor += bufferLength;
        }

        public static ISerializableObject DeserializeObject(AssetFile asset, ObjectExport export, Span<byte> uasset, Span<byte> uexp)
        {
            var blob = uexp.Length > 0 ? uexp : uasset;

            if (!ClassTypes.TryGetValue(export.ClassIndex.Name ?? "None", out var classType)) throw new NotImplementedException(export.ClassIndex.Name);

            if (!(Activator.CreateInstance(classType) is ISerializableObject instance)) throw new NotImplementedException(export.ClassIndex.Name);

            var cursor = (int) (uexp.Length > 0 ? export.SerialOffset - asset.Summary.TotalHeaderSize : export.SerialOffset);
            instance.Deserialize(blob, asset, ref cursor);
            return instance;
        }

        public static object? DeserializeStruct(Span<byte> buffer, AssetFile asset, string structName, ref int cursor)
        {
            if (structName == null) throw new InvalidDataException();
            switch (structName)
            {
                case nameof(Box):
                    return SpanHelper.ReadStruct<Box>(buffer, ref cursor);
                case nameof(Box2D):
                    return SpanHelper.ReadStruct<Box2D>(buffer, ref cursor);
                case nameof(Color):
                    return SpanHelper.ReadStruct<Color>(buffer, ref cursor);
                case nameof(IntPoint):
                    return SpanHelper.ReadStruct<IntPoint>(buffer, ref cursor);
                case nameof(LinearColor):
                    return SpanHelper.ReadStruct<LinearColor>(buffer, ref cursor);
                case nameof(Rotator):
                    return SpanHelper.ReadStruct<Rotator>(buffer, ref cursor);
                case nameof(Vector):
                    return SpanHelper.ReadStruct<Vector>(buffer, ref cursor);
                case nameof(Vector2D):
                    return SpanHelper.ReadStruct<Vector2D>(buffer, ref cursor);
                default:
                {
                    var obj = new UnrealObject();
                    obj.Deserialize(buffer, asset, ref cursor);
                    return obj;
                }
            }
        }

        public static bool IsSupported(ObjectExport export) => ClassTypes.ContainsKey(export.ClassIndex.Name ?? "None");
    }
}
