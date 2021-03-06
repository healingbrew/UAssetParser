using System;
using System.Collections.Generic;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Generics;

namespace UObject.ObjectModel
{
    [PublicAPI]
    public class DataTable : AbstractExportObject
    {
        public Dictionary<Name, UnrealObject> Data { get; set; } = new Dictionary<Name, UnrealObject>();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Deserialize(buffer, asset, ref cursor);
            asset.Stage = SerializationStage.Data;
            var count = Math.Abs(SpanHelper.ReadLittleInt(buffer, ref cursor));
            for (var i = 0; i < count; ++i)
            {
                var key = new Name();
                key.Deserialize(buffer, asset, ref cursor);
                var data = new UnrealObject();
                data.Deserialize(buffer, asset, ref cursor);
                Data[key] = data;
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Data.Count, ref cursor);
            foreach (var (key, value) in Data)
            {
                key.Serialize(ref buffer, asset, ref cursor);
                value.Serialize(ref buffer, asset, ref cursor);
            }
        }
    }
}
