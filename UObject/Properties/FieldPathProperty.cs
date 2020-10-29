using System;
using System.Collections.Generic;

using DragonLib.IO;

using JetBrains.Annotations;

using UObject.Asset;
using UObject.Enum;
using UObject.Generics;
using UObject.JSON;

namespace UObject.Properties
{
    [PublicAPI]
    public class FieldPathProperty : AbstractGuidProperty, IValueType<List<Name>>
    {
        public int Type { get; set; }
        public List<Name> Value { get; set; } = new List<Name>();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Deserialize(buffer, asset, ref cursor, mode);
            
            Type = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            
            for (var i = 0; i < count; ++i)
            {
                Value[i] = new Name();
                Value[i].Deserialize(buffer, asset, ref cursor);
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor, SerializationMode mode)
        {
            base.Serialize(ref buffer, asset, ref cursor);
            
            SpanHelper.WriteLittleInt(ref buffer, (int)Type, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Value.Count, ref cursor);
            
            foreach (var name in Value) {
                name.Serialize(ref buffer, asset, ref cursor);
            }
        }
    }
}
