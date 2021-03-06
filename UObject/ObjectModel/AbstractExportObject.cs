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
            #if DEBUG && UNVERSIONED_TEST
            if (asset.Summary.PackageFlags.HasFlag(PackageFlags.UnversionedProperties))
            {
                ExportData = new UnversionedObject();
            }
            else
            {
            #endif 
                ExportData.Deserialize(buffer, asset, ref cursor);
                Reserved = SpanHelper.ReadLittleInt(buffer, ref cursor);
            #if DEBUG && UNVERSIONED_TEST
            }
            #endif
        }

        public virtual void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ExportData.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, Reserved, ref cursor);
        }
    }
}
