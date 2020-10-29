using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using DragonLib.IO;
using JetBrains.Annotations;
using UObject.Generics;

namespace UObject.Asset
{
    [PublicAPI]
    public class PackageIndex : ISerializableObject
    {
        public int Index { get; set; }

        [JsonIgnore]
        public object? ObjectResource { get; set; }

        public bool IsImport => Index < 0;
        public bool IsExport => Index > 0;
        public bool IsNull => Index == 0;

        public string? Name => ObjectResource switch
        {
            ObjectExport export => export.ObjectName,
            ObjectImport import => import.ObjectName,
            _ => null
        };

        public string? FullName => ObjectResource switch
        {
            ObjectExport export => export.ObjectName,
            ObjectImport import => import.PackageRef.IsNull ? import.ObjectName : import.PackageRef.Name,
            _ => null
        };

        public void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            Debug.WriteLineIf(Debugger.IsAttached, $"Deserialize called for {nameof(PackageIndex)} at {cursor:X}");
            Index = SpanHelper.ReadLittleInt(buffer, ref cursor);
            var importIndex = 0 - Index - 1;
            var exportIndex = Index - 1;
            if (IsImport && asset.Imports.Length > importIndex) ObjectResource = asset.Imports[importIndex];
            if (IsExport && asset.Exports.Length > exportIndex) ObjectResource = asset.Exports[exportIndex];
        }

        public void Serialize(ref Memory<byte> buffer, AssetFile asset, ref int cursor) => SpanHelper.WriteLittleInt(ref buffer, Index, ref cursor);
    }
}
