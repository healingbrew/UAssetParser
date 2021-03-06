using System;

using DragonLib.IO;
using JetBrains.Annotations;
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
            if (asset.Summary.PackageFlags.HasFlag(PackageFlags.UnversionedProperties))
            {
                throw new NotSupportedException("Unversioned UObject Properties are not supported");
            }
            ExportData.Deserialize(buffer, asset, ref cursor);
            Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
        }

        public virtual void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Reserved, ref cursor);
        }
    }
}
