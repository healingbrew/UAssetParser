using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.EndGame.Properties;
using UObject.ObjectModel;

namespace UObject.EndGame.ObjectModel
{
    [PublicAPI]
    public class EndTextResource : AbstractExportObject
    {
        public Dictionary<string, EndTextProperty> Data { get; set; } = new Dictionary<string, EndTextProperty>();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Deserialize(buffer, asset, ref cursor);
            asset.Stage = SerializationStage.Data;
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            for (var i = 0; i < count; ++i)
            {
                var key = ObjectSerializer.DeserializeString(buffer, ref cursor);
                if (string.IsNullOrEmpty(key) || key[0] != '$')
                    throw new InvalidDataException("The key does not start with magic symbol");
                var resource = new EndTextProperty();
                resource.Deserialize(buffer, asset, ref cursor);
                Data.Add(key, resource);
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => throw new NotImplementedException();
    }
}
