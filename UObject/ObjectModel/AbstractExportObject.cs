using System;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using System.Collections.Generic;
using UObject.Asset;
using UObject.Generics;

namespace UObject.ObjectModel
{
    [PublicAPI]
    public class AbstractExportObject : ISerializableObject
    {
        public int                         Reserved   { get; set; }
        public UnrealObject                ExportData { get; set; } = new UnrealObject();

        public virtual void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            asset.Stage = SerializationStage.Instance;
            #if DEBUG
            if (asset.Summary.PackageFlags.HasFlag(PackageFlags.UnversionedProperties))
            {
                ExportData = new UnversionedObject();
            }
            #endif
            ExportData.Deserialize(buffer, asset, ref cursor);
            Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
        }

        public virtual void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Reserved, ref cursor);
        }
    }

    public class UnversionedObject : UnrealObject
    {
        public List<UnversionedFragment> Fragments { get; set; } = new List<UnversionedFragment>();
        public byte[] MaskBytes { get; set; } = Array.Empty<byte>();
        
        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            var fragment = new UnversionedFragment();
            var mask = 0;
            while (!fragment.IsLast)
            {
                fragment = new UnversionedFragment();
                fragment.Deserialize(buffer, asset, ref cursor);
                if (fragment.HasZero) mask += fragment.Count;
                Fragments.Add(fragment);
            }

            if (mask > 0)
            {
                if (mask <= 8)
                {
                    MaskBytes = buffer.Slice(cursor, 1).ToArray();
                }
                else if (mask <= 16)
                {
                    MaskBytes = buffer.Slice(cursor, 2).ToArray();
                }
                else
                {
                    MaskBytes = buffer.Slice(cursor, ((mask + 31) >> 5) * 4).ToArray();
                }
            }
        }
    }
}
