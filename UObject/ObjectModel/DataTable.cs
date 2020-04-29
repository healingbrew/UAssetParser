﻿using System;
using System.Collections.Generic;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Asset;
using UObject.Properties;

namespace UObject.ObjectModel
{
    [PublicAPI]
    public class DataTable : UnrealObject
    {
        public Dictionary<string, UnrealObject> Table { get; set; } = new Dictionary<string, UnrealObject>();

        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            base.Deserialize(buffer, asset, ref cursor);
            cursor += 4;
            var count = SpanHelper.ReadLittleInt(buffer, ref cursor);
            for (var i = 0; i < count; ++i)
            {
                var key = ObjectSerializer.DeserializeProperty<Name>(buffer, asset, ref cursor);
                var data = ObjectSerializer.DeserializeProperty<UnrealObject>(buffer, asset, ref cursor);
                Table[key] = data;
            }
        }

        public override void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => throw new NotImplementedException();
    }
}
