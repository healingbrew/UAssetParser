using System;
using System.Collections.Generic;
using System.IO;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.GameModel.FF7.Properties;
using UObject.Generics;
using UObject.ObjectModel;

namespace UObject.GameModel.FF7.ObjectModel
{
    [PublicAPI]
    public class EndTextResource : ISerializableObject
    {
        public int Reserved { get; set; }
        public Dictionary<string, FF7TxtRes> Data { get; set; } = new Dictionary<string, FF7TxtRes>();
        public UnrealObject ExportData { get; set; } = new UnrealObject();

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Deserialize(buffer, asset, ref cursor);
            Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            for (var i = 0; i < count; ++i)
            {
                var key = ObjectSerializer.DeserializeString(buffer, ref cursor);
                if (string.IsNullOrEmpty(key) || key[0] != '$')
                    throw new InvalidDataException("The key does not start with magic symbol");
                var resource = new FF7TxtRes();
                resource.Deserialize(buffer, asset, ref cursor);
                Data.Add(key, resource);
            }
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => throw new NotImplementedException();
    }
}
