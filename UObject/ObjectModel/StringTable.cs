using System;
using System.Collections.Generic;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;

namespace UObject.ObjectModel
{
    [PublicAPI]
    public class StringTable : AbstractExportObject
    {
        public string? Name { get; set; }
        public Dictionary<string, string?> Data { get; set; } = new Dictionary<string, string?>();
        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Deserialize(buffer, asset, ref cursor);
            asset.Stage = SerializationStage.Data;
            Name = ObjectSerializer.DeserializeString(buffer, ref cursor);
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            for (var i = 0; i < count; ++i) Data.Add(ObjectSerializer.DeserializeString(buffer, ref cursor) ?? $"{cursor:X}", ObjectSerializer.DeserializeString(buffer, ref cursor));
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Data.Count, ref cursor);
            foreach (var (key, value) in Data)
            {
                ObjectSerializer.SerializeString(ref buffer, key, ref cursor);
                ObjectSerializer.SerializeString(ref buffer, value ?? String.Empty, ref cursor);
            }
        }
    }
}
