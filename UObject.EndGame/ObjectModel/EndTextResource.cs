using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.EndGame.Properties;
using UObject.Generics;
using UObject.ObjectModel;

namespace UObject.EndGame.ObjectModel
{
    [PublicAPI]
    public class EndTextResource : ISerializableObject
    {
        public int Reserved { get; set; }
        public Dictionary<string, EndTextProperty> Data { get; set; } = new Dictionary<string, EndTextProperty>();
        public UnrealObject ExportData { get; set; } = new UnrealObject();

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(EndTextResource)} at {cursor:X}");
            ExportData.Deserialize(buffer, asset, ref cursor);
            Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
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

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => throw new NotImplementedException();
    }
}
