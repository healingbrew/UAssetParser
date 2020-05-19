using System;
using System.Collections.Generic;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Generics;

namespace UObject.ObjectModel
{
    [PublicAPI]
    public class StringTable : ISerializableObject
    {
        public string? Name { get; set; }
        public int Reserved { get; set; }
        public Dictionary<string, string?> Data { get; set; } = new Dictionary<string, string?>();
        public UnrealObject ExportData { get; set; } = new UnrealObject();

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Deserialize(buffer, asset, ref cursor);
            Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
            Name = ObjectSerializer.DeserializeString(buffer, ref cursor);
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            for (var i = 0; i < count; ++i) Data.Add(ObjectSerializer.DeserializeString(buffer, ref cursor) ?? $"{cursor:X}", ObjectSerializer.DeserializeString(buffer, ref cursor));
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Reserved, ref cursor);
            ObjectSerializer.SerializeString(ref buffer, Name ?? String.Empty, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Data.Count, ref cursor);
            foreach (var (key, value) in Data)
            {
                ObjectSerializer.SerializeString(ref buffer, key, ref cursor);
                ObjectSerializer.SerializeString(ref buffer, value ?? String.Empty, ref cursor);
            }
        }
    }
}
