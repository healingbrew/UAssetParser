using System;
using System.Collections.Generic;

using DragonLib.IO;

using JetBrains.Annotations;

using UObject.Asset;
using UObject.Generics;
using UObject.JSON;

namespace UObject.Structs
{
    [PublicAPI]
    public class GameplayTagContainer : ISerializableObject, IValueType<List<Name>>
    {
        public List<Name> Value { get; set; } = new List<Name>();
        
        public void   Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            Value = new List<Name>(count);

            for (var i = 0; i < count; ++i)
            {
                var name = new Name();
                name.Deserialize(buffer, asset, ref cursor);
                Value.Add(name);
            }
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => throw new NotImplementedException();
    }
}
