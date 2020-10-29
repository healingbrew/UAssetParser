using System;
using System.Diagnostics;

using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Enum;
using UObject.Generics;

namespace UObject.Asset
{
    [PublicAPI]
    public class ObjectExport : ISerializableObject
    {
        public PackageIndex ClassIndex { get; set; } = new PackageIndex();
        public PackageIndex SuperIndex { get; set; } = new PackageIndex();
        public PackageIndex TemplateIndex { get; set; } = new PackageIndex();
        public PackageIndex OuterIndex { get; set; } = new PackageIndex();
        public Name ObjectName { get; set; } = new Name();
        public ObjectFlags ObjectFlags { get; set; }
        public long SerialSize { get; set; }
        public long SerialOffset { get; set; }
        public bool ForcedExport { get; set; }
        public bool NotForClient { get; set; }
        public bool NotForServer { get; set; }
        public Guid PackageGuid { get; set; }
        public uint PackageFlags { get; set; }
        public bool NotAlwaysLoadedForEditorGame { get; set; }
        public bool IsAsset { get; set; }
        public int FirstExportDependency { get; set; }
        public bool SerializationBeforeSerializationDependencies { get; set; }
        public bool CreateBeforeSerializationDependencies { get; set; }
        public bool SerializationBeforeCreateDependencies { get; set; }
        public bool CreateBeforeCreateDependencies { get; set; }
        public DynamicType DynamicType { get; set; }

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(ObjectExport)} at {cursor:X}");
            ClassIndex.Deserialize(buffer, asset, ref cursor);
            SuperIndex.Deserialize(buffer, asset, ref cursor);
            TemplateIndex.Deserialize(buffer, asset, ref cursor);
            OuterIndex.Deserialize(buffer, asset, ref cursor);
            ObjectName.Deserialize(buffer, asset, ref cursor);
            ObjectFlags = (ObjectFlags) SpanHelper.ReadLittleUInt(buffer, ref cursor);
            SerialSize = asset.Summary.FileVersionUE4 >= 511 ? SpanHelper.ReadLittleLong(buffer, ref cursor) : SpanHelper.ReadLittleInt(buffer, ref cursor);
            SerialOffset = asset.Summary.FileVersionUE4 >= 511 ? SpanHelper.ReadLittleLong(buffer, ref cursor) : SpanHelper.ReadLittleInt(buffer, ref cursor);
            ForcedExport = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            NotForClient = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            NotForServer = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            PackageGuid = SpanHelper.ReadStruct<Guid>(buffer, ref cursor);
            PackageFlags = SpanHelper.ReadLittleUInt(buffer, ref cursor);
            NotAlwaysLoadedForEditorGame = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            IsAsset = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            FirstExportDependency = SpanHelper.ReadLittleInt(buffer, ref cursor);
            SerializationBeforeSerializationDependencies = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            CreateBeforeSerializationDependencies = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            SerializationBeforeCreateDependencies = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            CreateBeforeCreateDependencies = SpanHelper.ReadLittleInt(buffer, ref cursor) == 1;
            DynamicType = (DynamicType) SpanHelper.ReadLittleUInt(buffer, ref cursor);
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor)
        {
            ClassIndex.Serialize(ref buffer, asset, ref cursor);
            SuperIndex.Serialize(ref buffer, asset, ref cursor);
            TemplateIndex.Serialize(ref buffer, asset, ref cursor);
            OuterIndex.Serialize(ref buffer, asset, ref cursor);
            ObjectName.Serialize(ref buffer, asset, ref cursor);
            SpanHelper.WriteLittleUInt(ref buffer, (uint) ObjectFlags, ref cursor);
            SpanHelper.WriteLittleLong(ref buffer, SerialSize, ref cursor);
            SpanHelper.WriteLittleLong(ref buffer, SerialOffset, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, ForcedExport ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, NotForClient ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, NotForServer ? 1 : 0, ref cursor);
            SpanHelper.WriteStruct(ref buffer, PackageGuid, ref cursor);
            SpanHelper.WriteLittleUInt(ref buffer, PackageFlags, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, NotAlwaysLoadedForEditorGame ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, IsAsset ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, FirstExportDependency, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, SerializationBeforeSerializationDependencies ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, CreateBeforeSerializationDependencies ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, SerializationBeforeCreateDependencies ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleInt(ref buffer, CreateBeforeCreateDependencies ? 1 : 0, ref cursor);
            SpanHelper.WriteLittleUInt(ref buffer, (uint) DynamicType, ref cursor);
        }
    }
}
