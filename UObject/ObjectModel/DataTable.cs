using System;
using System.Collections.Generic;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Generics;

namespace UObject.ObjectModel
{
    [PublicAPI]
    public class DataTable : ISerializableObject
    {
        public Dictionary<Name, UnrealObject> Data { get; set; } = new Dictionary<Name, UnrealObject>();
        public int Reserved { get; set; }
        public UnrealObject ExportData { get; set; } = new UnrealObject();

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Deserialize(buffer, asset, ref cursor);
            Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            for (var i = 0; i < count; ++i)
            {
                var key = new Name();
                key.Deserialize(buffer, asset, ref cursor);
                var data = new UnrealObject();
                data.Deserialize(buffer, asset, ref cursor);
                Data[key] = data;
            }
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Reserved, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Data.Count, ref cursor);
            foreach (var obj in Data)
            {
                obj.Key.Serialize(ref buffer, asset, ref cursor);
                obj.Value.Serialize(ref buffer, asset, ref cursor);
            }
        }
    }
}
