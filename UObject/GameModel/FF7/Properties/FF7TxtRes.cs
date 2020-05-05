using System;
using System.Collections.Generic;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Generics;
using UObject.JSON;

namespace UObject.GameModel.FF7.Properties
{
    [PublicAPI]
    public class FF7TxtRes: IArrayValueType<Dictionary<Name,string?>?>
    {
        public string? Str { get; set; }
        public Dictionary<Name, string?>? Value { get; set; } 

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Str = ObjectSerializer.DeserializeString(buffer, ref cursor);
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            if (count > 0)
                Value = new Dictionary<Name, string?>();
            for (int i = 0; i < count; i++)
            {
                var fname = new Name();
                fname.Deserialize(buffer, asset, ref cursor);
                string? data = ObjectSerializer.DeserializeString(buffer, ref cursor);
                Value?.Add(fname, data);
            }
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
        }

       
    }
}
